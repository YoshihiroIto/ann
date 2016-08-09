using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ann.Foundation;
using Ann.Foundation.Exception;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Core
{
    public class App : DisposableModelBase
    {
        private static App _Instance;

        public static App Instance
        {
            get
            {
                if (_Instance == null)
                    throw new UninitializedException();

                return _Instance;
            }
        }

        #region Candidates

        private IEnumerable<ExecutableUnit> _Candidates = Enumerable.Empty<ExecutableUnit>();

        public IEnumerable<ExecutableUnit> Candidates
        {
            get { return _Candidates; }
            set { SetProperty(ref _Candidates, value); }
        }

        #endregion

        public Config.App Config { get; private set; }
        public Config.MostRecentUsedList MruList { get; private set; }

        public event EventHandler PriorityFilesChanged;
        public event EventHandler ShortcutKeyChanged;

        public void InvokePriorityFilesChanged() => PriorityFilesChanged?.Invoke(this, EventArgs.Empty);
        public void InvokeShortcutKeyChanged() => ShortcutKeyChanged?.Invoke(this, EventArgs.Empty);

        private HashSet<string> _priorityFiles = new HashSet<string>();
        private readonly ExecutableUnitDataBase _dataBase;
        private readonly InputControler _inputControler;

        private static string IndexFilePath => System.IO.Path.Combine(
            Constants.ConfigDirPath,
            $"{(Foundation.TestHelper.IsTestMode ? "Test." : string.Empty)}index.dat");

        #region IndexOpeningResult

        private IndexOpeningResults _IndexOpeningResult;

        public IndexOpeningResults IndexOpeningResult
        {
            get { return _IndexOpeningResult; }
            set { SetProperty(ref _IndexOpeningResult, value); }
        }

        #endregion

        public static void Clean()
        {
            _Instance?.Dispose();
            _Instance = null;
        }

        public static void Initialize()
        {
            if (_Instance != null)
                throw new NestingException();

            _Instance = new App();
        }

        public static void Destory()
        {
            if (_Instance == null)
                throw new NestingException();

            Clean();
        }

        public static void RemoveIndexFile()
        {
            if (File.Exists(IndexFilePath))
                File.Delete(IndexFilePath);
        }

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
                }

                return Config.TargetFolder.Folders.Select(x => x.Value)
                    .Concat(folders);
            }
        }

        public async Task OpenIndexAsync()
        {
            IndexOpeningResult = IndexOpeningResults.InOpening;
            IndexOpeningResult = await _dataBase.OpenIndexAsync(TagetFolders);
        }

        public async Task UpdateIndexAsync()
        {
            IndexOpeningResult = await _dataBase.UpdateIndexAsync(TagetFolders, Config.ExecutableFileExts);
        }

        public void Find(string input, int maxCandidates)
        {
            _inputControler.Push(() =>
            {
                Candidates = _dataBase
                    .Find(input, Config.ExecutableFileExts)
                    .OrderBy(u => MakeOrder(u.Path))
                    .Take(maxCandidates)
                    .ToArray();
            });
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

                    SaveMru();

                    _mruOrders.Clear();
                    MruList.AppPath.ForEach((path, index) => _mruOrders[path] = index);
                });
            }

            return i;
        }

        private App()
        {
            _dataBase = new ExecutableUnitDataBase(IndexFilePath);
            _inputControler = new InputControler().AddTo(CompositeDisposable);

            LoadConfig();

            SetupAutoUpdater();

            Task.Run(async () =>
            {
                if (Config.IsStartOnSystemStartup)
                    await VersionUpdater.Instance.CreateStartupShortcut();
                else
                    await VersionUpdater.Instance.RemoveStartupShortcut();
            });
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
            Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromHours(12))
                .ObserveOnUIDispatcher()
                .Subscribe(async _ =>
                {
                    await VersionUpdater.Instance.CheckAsync();

                    if (VersionUpdater.Instance.IsAvailableUpdate)
                    {
                        if (IsEnableAutoUpdater)
                        {
                            AutoUpdateState = AutoUpdateStates.CloseAfterNSec;
                            AutoUpdateRemainingSeconds = Constants.AutoUpdateCloseDelaySec;

                            while (AutoUpdateRemainingSeconds > 0)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                -- AutoUpdateRemainingSeconds;
                            }

                            await Task.Delay(TimeSpan.FromSeconds(2));

                            VersionUpdater.Instance.RequestRestart();
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

        #region AutoUpdateState

        private AutoUpdateStates _AutoUpdateState;

        public AutoUpdateStates AutoUpdateState
        {
            get { return _AutoUpdateState; }
            private set { SetProperty(ref _AutoUpdateState, value); }
        }

        #endregion

        #region config

        private void LoadConfig()
        {
            Debug.Assert(Config == null);
            Debug.Assert(MruList == null);

            {
                Config = ConfigHelper.ReadConfig<Config.App>(ConfigHelper.Category.App, Constants.ConfigDirPath);

                RefreshPriorityFiles();

                Config.PriorityFiles.ObserveAddChanged()
                    .Subscribe(p =>
                    {
                        _priorityFiles.Add(p.Value.ToLower());
                        SaveConfig();
                        InvokePriorityFilesChanged();
                    })
                    .AddTo(CompositeDisposable);

                Config.PriorityFiles.ObserveRemoveChanged()
                    .Subscribe(p =>
                    {
                        _priorityFiles.Remove(p.Value.ToLower());
                        SaveConfig();
                        InvokePriorityFilesChanged();
                    })
                    .AddTo(CompositeDisposable);
            }

            {
                MruList = ConfigHelper.ReadConfig<Config.MostRecentUsedList>(ConfigHelper.Category.MostRecentUsedList,
                    Constants.ConfigDirPath);
                MruList.AppPath.ForEach((p, index) => _mruOrders[p] = index);
            }
        }

        public void SaveConfig()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.App, Constants.ConfigDirPath, Config);
        }

        public void SaveMru()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.MostRecentUsedList, Constants.ConfigDirPath, MruList);
        }

        #endregion
    }
}