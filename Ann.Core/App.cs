using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;
using System.Threading;
using Ann.Core.Candidate;
using Reactive.Bindings;

namespace Ann.Core
{
    public class App : DisposableModelBase
    {
        #region Candidates

        private IEnumerable<ICandidate> _Candidates = Enumerable.Empty<ICandidate>();

        public IEnumerable<ICandidate> Candidates
        {
            get { return _Candidates; }
            set { SetProperty(ref _Candidates, value); }
        }

        #endregion

        #region CrawlingCount

        private int _Crawling;

        public int Crawling
        {
            get { return _Crawling; }
            set { SetProperty(ref _Crawling, value); }
        }

        #endregion

        private readonly ConfigHolder _configHolder;
        private Config.App Config => _configHolder.Config;
        private Config.MostRecentUsedList MruList => _configHolder.MruList;

        private readonly LanguagesService _languagesService;

        public event EventHandler PriorityFilesChanged;
        public event EventHandler ShortcutKeyChanged;

        public void InvokePriorityFilesChanged() => PriorityFilesChanged?.Invoke(this, EventArgs.Empty);
        public void InvokeShortcutKeyChanged() => ShortcutKeyChanged?.Invoke(this, EventArgs.Empty);

        private readonly InputQueue _inputQueue;
        private readonly ExecutableFileDataBase _executableFileDataBase;
        private readonly Calculator _calculator = new Calculator();
        private readonly Translator _translator;

        private HashSet<string> _priorityFiles = new HashSet<string>();

        private string IndexFilePath => System.IO.Path.Combine(_configHolder.ConfigDirPath, "index.dat");

        public VersionUpdater VersionUpdater { get; }

        #region IndexOpeningResult

        private IndexOpeningResults _IndexOpeningResult;

        public IndexOpeningResults IndexOpeningResult
        {
            get { return _IndexOpeningResult; }
            set { SetProperty(ref _IndexOpeningResult, value); }
        }

        #endregion

        #region IsIndexUpdating

        private bool _IsIndexUpdating;

        public bool IsIndexUpdating
        {
            get { return _IsIndexUpdating; }
            set { SetProperty(ref _IsIndexUpdating, value); }
        }

        #endregion

        #region IsEnableActivateHotKey

        private bool _IsEnableActivateHotKey = true;

        public bool IsEnableActivateHotKey
        {
            get { return _IsEnableActivateHotKey; }
            set { SetProperty(ref _IsEnableActivateHotKey, value); }
        }

        #endregion

        public bool IsPriorityFile(string path) => _priorityFiles.Contains(path.ToLower());

        public bool AddPriorityFile(string path)
        {
            if (_priorityFiles.Contains(path.ToLower()))
                return false;

            Config.PriorityFiles.Add(new Path(path));
            return true;
        }

        public bool RemovePriorityFile(string path)
        {
            if (_priorityFiles.Contains(path.ToLower()) == false)
                return false;

            var found = Config.PriorityFiles.First(p => p.Value == path);
            Config.PriorityFiles.Remove(found);
            return true;
        }

        public IEnumerable<string> TagetFolders
        {
            get
            {
                var folders = new List<string>();
                {
                    if (Config.TargetFolder.IsIncludeSystemFolder)
                        folders.Add(Constants.SystemFolder);

                    if (Config.TargetFolder.IsIncludeSystemX86Folder)
                        folders.Add(Constants.SystemX86Folder);

                    if (Config.TargetFolder.IsIncludeProgramsFolder)
                        folders.Add(Constants.ProgramsFolder);

                    if (Config.TargetFolder.IsIncludeProgramFilesFolder)
                        folders.Add(Constants.ProgramFilesFolder);

                    if (Config.TargetFolder.IsIncludeProgramFilesX86Folder)
                        folders.Add(Constants.ProgramFilesX86Folder);

                    if (Config.TargetFolder.IsIncludeCommonStartMenu)
                        folders.Add(Constants.CommonStartMenuFolder);
                }

                return Config.TargetFolder.Folders.Select(x => x.Value)
                    .Concat(folders)
                    .Distinct();
            }
        }

        public async Task OpenIndexAsync()
        {
            IndexOpeningResult = IndexOpeningResults.InOpening;
            IndexOpeningResult = await _executableFileDataBase.OpenIndexAsync(TagetFolders);
        }

        private readonly SemaphoreSlim _CancelUpdateIndexAsyncSema = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _UpdateIndexAsyncSema = new SemaphoreSlim(1, 1);

        public async Task CancelUpdateIndexAsync()
        {
            using (Disposable.Create(() => _CancelUpdateIndexAsyncSema.Release()))
            {
                await _CancelUpdateIndexAsyncSema.WaitAsync();
                await _executableFileDataBase.CancelUpdateIndexAsync();

                while (IsIndexUpdating)
                    await Task.Delay(TimeSpan.FromMilliseconds(20));
            }
        }

        public void CancelUpdateIndex()
        {
            Task.Run(CancelUpdateIndexAsync);
        }

        public async Task UpdateIndexAsync()
        {
            await CancelUpdateIndexAsync();

            using (Disposable.Create(() => _UpdateIndexAsyncSema.Release()))
            {
                await _UpdateIndexAsyncSema.WaitAsync();

                using (Disposable.Create(() => IsIndexUpdating = false))
                {
                    IsIndexUpdating = true;
                    IndexOpeningResult =
                        await _executableFileDataBase.UpdateIndexAsync(TagetFolders, Config.ExecutableFileExts);
                }
            }
        }

        private Subject<string> _translatorSubject;
        private string _currentInput;

        private void SetupTranslatorSubject()
        {
            _translatorSubject = new Subject<string>().AddTo(CompositeDisposable);
            _translatorSubject
                .Throttle(TimeSpan.FromMilliseconds(150))
                .Subscribe(input =>
                {
                    var parts = input.Split(' ');
                    var keyword = parts[0];

                    var translatorSet = Config.Translator.TranslatorSet.First(x => x.Keyword.ToLower() == keyword);

                    var r = _translator.TranslateAsync(
                        input.Substring(keyword.Length),
                        translatorSet.From,
                        translatorSet.To).Result;

                    if (input == _currentInput)
                        Candidates = r != null ? new[] {r} : new ICandidate[0];
                }).AddTo(CompositeDisposable);
        }

        public void Find(string input)
        {
            _currentInput = input;

            if (input == null)
                return;

            input = input.Trim();
            _currentInput = input;

            _inputQueue.Push(() =>
            {
                switch (MakeCommand(input))
                {
                    case CommandType.Nothing:
                    {
                        Candidates = new ICandidate[0];
                        break;
                    }

                    case CommandType.Translate:
                    {
                        if (input.Split(' ').Length >= 2)
                            _translatorSubject.OnNext(input);
                        else
                            Candidates = new ICandidate[0];

                        break;
                    }

                    case CommandType.Calculate:
                    {
                        var r = _calculator.Calculate(this, input);
                        Candidates = r != null ? new[] {r} : new ICandidate[0];

                        break;
                    }

                    case CommandType.ExecutableFile:
                    {
                        Candidates = _executableFileDataBase
                            .Find(input, Config.ExecutableFileExts)
                            .OrderBy(u => MakeOrder(u.Path))
                            .Take(Config.MaxCandidateLinesCount)
                            .ToArray();

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private enum CommandType
        {
            Nothing,
            Translate,
            Calculate,
            ExecutableFile
        }

        private CommandType MakeCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return CommandType.Nothing;

            var langs = Config.Translator.TranslatorSet.Select(x => x.Keyword.ToLower());
            if (langs.Any(l => input.StartsWith(l + " ")))
                return CommandType.Translate;

            if (Calculator.CanAccepte(input))
                return CommandType.Calculate;

            return CommandType.ExecutableFile;
        }

        private const int MaxMruCount = 50;
        private readonly Dictionary<string, int> _mruOrders = new Dictionary<string, int>();

        private int MakeOrder(string path)
        {
            if (IsPriorityFile(path))
                return 0;

            int order;
            if (_mruOrders.TryGetValue(path, out order))
                return 1 + order;

            return 1 + MaxMruCount;
        }

        public void RefreshPriorityFiles()
        {
            _priorityFiles = new HashSet<string>(Config.PriorityFiles.Select(p => p.Value.ToLower()));
        }

        public async Task<bool> RunAsync(string appPath, bool isRunAsAdmin)
        {
            var i = await ProcessHelper.RunAsync(appPath, string.Empty, isRunAsAdmin);

            if (i)
            {
                await Task.Run(() =>
                {
                    MruList.AppPath.Remove(appPath);
                    MruList.AppPath.Insert(0, appPath);

                    while (MruList.AppPath.Count > MaxMruCount)
                        MruList.AppPath.RemoveAt(MruList.AppPath.Count - 1);

                    _configHolder.SaveMru();

                    _mruOrders.Clear();
                    MruList.AppPath.ForEach((path, index) => _mruOrders[path] = index);
                });
            }

            return i;
        }

        public string GetString(StringTags tag) => _languagesService.GetString(tag);

        public App(ConfigHolder configHolder, LanguagesService languagesService)
        {
            Debug.Assert(configHolder != null);
            Debug.Assert(languagesService != null);

            _configHolder = configHolder;
            _languagesService = languagesService;

            _inputQueue = new InputQueue().AddTo(CompositeDisposable);

            UpdateFromConfig();

            _executableFileDataBase = new ExecutableFileDataBase(this, IndexFilePath);
            _executableFileDataBase.ObserveProperty(x => x.CrawlingCount)
                .Subscribe(c => Crawling = c)
                .AddTo(CompositeDisposable);

            _translator = new Translator(
                this,
                configHolder.Config.Translator.MicrosoftTranslatorClientId,
                configHolder.Config.Translator.MicrosoftTranslatorClientSecret
            );

            SetupTranslatorSubject();

            VersionUpdater = new VersionUpdater(Config.GitHubPersonalAccessToken).AddTo(CompositeDisposable);

            SetupAutoUpdater();

            Task.Run(async () =>
            {
                if (Config.IsStartOnSystemStartup)
                    await VersionUpdater.CreateStartupShortcut();
                else
                    await VersionUpdater.RemoveStartupShortcut();
            });

            CompositeDisposable.Add(CancelUpdateIndex);
        }

        private void UpdateFromConfig()
        {
            {
                RefreshPriorityFiles();

                Config.PriorityFiles.ObserveAddChanged()
                    .Subscribe(p =>
                    {
                        _priorityFiles.Add(p.Value.ToLower());
                        _configHolder.SaveConfig();
                        InvokePriorityFilesChanged();
                    })
                    .AddTo(CompositeDisposable);

                Config.PriorityFiles.ObserveRemoveChanged()
                    .Subscribe(p =>
                    {
                        _priorityFiles.Remove(p.Value.ToLower());
                        _configHolder.SaveConfig();
                        InvokePriorityFilesChanged();
                    })
                    .AddTo(CompositeDisposable);
            }

            {
                MruList.AppPath.ForEach((p, index) => _mruOrders[p] = index);
            }
        }

        public bool IsEnableAutoUpdater { get; set; }

        #region AutoUpdateRemainingSeconds

        private int _AutoUpdateRemainingSeconds;

        public int AutoUpdateRemainingSeconds
        {
            get { return _AutoUpdateRemainingSeconds; }
            private set { SetProperty(ref _AutoUpdateRemainingSeconds, value); }
        }

        #endregion

        private void SetupAutoUpdater()
        {
            if (VersionUpdater.IsEnableSilentUpdate == false)
                return;

            Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromHours(8))
                .ObserveOn(ReactivePropertyScheduler.Default)
                .Subscribe(async _ =>
                {
                    await VersionUpdater.CheckAsync();

                    if (VersionUpdater.IsAvailableUpdate)
                    {
                        if (IsEnableAutoUpdater)
                        {
                            AutoUpdateState = AutoUpdateStates.CloseAfterNSec;
                            AutoUpdateRemainingSeconds = Constants.AutoUpdateCloseDelaySec;

                            while (AutoUpdateRemainingSeconds > 0)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                --AutoUpdateRemainingSeconds;
                            }

                            await Task.Delay(TimeSpan.FromSeconds(2));

                            VersionUpdater.RequestRestart();
                            Application.Current.MainWindow.Close();
                        }
                    }
                }).AddTo(CompositeDisposable);
        }

        public enum AutoUpdateStates
        {
            Wait,
            CloseAfterNSec
        }

        public bool IsRestartRequested => VersionUpdater.IsRestartRequested;

        #region AutoUpdateState

        private AutoUpdateStates _AutoUpdateState;

        public AutoUpdateStates AutoUpdateState
        {
            get { return _AutoUpdateState; }
            private set { SetProperty(ref _AutoUpdateState, value); }
        }

        #endregion

        public int ExecutableFileDataBaseIconCacheSize
        {
            get { return _executableFileDataBase.IconCacheSize; }
            set { _executableFileDataBase.IconCacheSize = value; }
        }

        public class NotificationEventArgs : EventArgs
        {
            public StringTags[] Messages;
        }

        public event EventHandler<NotificationEventArgs> Notification;

        public void NoticeMessages(IEnumerable<StringTags> messages)
        {
            Notification?.Invoke(this, new NotificationEventArgs
            {
                Messages = messages.ToArray()
            });
        }
    }
}