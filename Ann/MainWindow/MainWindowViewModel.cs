using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; }

        public ReactiveProperty<bool> IsIndexUpdating { get; }
        public ReactiveCommand IndexUpdateCommand { get; }

        public ReactiveCommand InitializeCommand { get; }

        public ReactiveProperty<ExecutableUnitViewModel[]> Candidates { get; }
        public ReactiveProperty<ExecutableUnitViewModel> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand<string> RunCommand { get; }
        public ReactiveCommand ContainingFolderOpenCommand { get; }

        public ReactiveCommand ShowCommand { get; }
        public ReactiveCommand HideCommand { get; }
        public ReactiveCommand ExitCommand { get; }

        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<int> MaxCandidatesLinesCount { get; }

        public ImageSource GetIcon(string path) => _iconDecoder.GetIcon(path);
        public ReadOnlyReactiveProperty<double> CandidatesListMaxHeight { get; }
        public ReactiveProperty<double> CandidateItemHeight { get; }

        public AsyncReactiveCommand SettingShowCommand { get; }
        public ReactiveProperty<bool> IsShowingSettingShow { get; }

        public ReadOnlyReactiveProperty<IndexOpeningResults> IndexOpeningResult { get; }
        public ReactiveProperty<bool> IsEnableActivateHotKey { get; }

        public WindowMessageBroker Messenger { get; }

        public Core.Config.MainWindow Config { get; private set; }

        public string Caption { get; } = $"{AssemblyConstants.Product} {AssemblyConstants.Version}";

        private readonly IconDecoder _iconDecoder = new IconDecoder();

        public StatusBarViewModel StatusBar { get; }

        public MainWindowViewModel()
        {
            using (new TimeMeasure("MainWindowViewModel.ctor"))
            {
                LoadConfig();

                Messenger = new WindowMessageBroker().AddTo(CompositeDisposable);

                InitializeCommand = new ReactiveCommand().AddTo(CompositeDisposable);
                InitializeCommand
                    .Subscribe(async _ => await InitializeAsync())
                    .AddTo(CompositeDisposable);

                Left = Config.ToReactivePropertyAsSynchronized(x => x.Left).AddTo(CompositeDisposable);
                Top = Config.ToReactivePropertyAsSynchronized(x => x.Top).AddTo(CompositeDisposable);

                Input = new ReactiveProperty<string>().AddTo(CompositeDisposable);
                IsIndexUpdating = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

                MaxCandidatesLinesCount = App.Instance.Config
                    .ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                    .AddTo(CompositeDisposable);

                CandidateItemHeight = new ReactiveProperty<double>().AddTo(CompositeDisposable);
                CandidatesListMaxHeight =
                    Observable
                        .Merge(MaxCandidatesLinesCount.ToUnit())
                        .Merge(CandidateItemHeight.ToUnit())
                        .Select(_ => CandidateItemHeight.Value*MaxCandidatesLinesCount.Value + 4)
                        .ToReadOnlyReactiveProperty()
                        .AddTo(CompositeDisposable);

                IndexUpdateCommand = IsIndexUpdating.Select(i => i == false)
                    .ToReactiveCommand()
                    .AddTo(CompositeDisposable);

                IndexUpdateCommand
                    .Subscribe(async _ => await UpdateIndexAsync())
                    .AddTo(CompositeDisposable);

                Observable
                    .Merge(Input.ToUnit())
                    .Merge(App.Instance.Config.ObserveProperty(x => x.CandidatesCensoringSize).ToUnit())
                    .Throttle(TimeSpan.FromMilliseconds(50))
                    .Subscribe(_ => App.Instance.Find(Input.Value, App.Instance.Config.CandidatesCensoringSize))
                    .AddTo(CompositeDisposable);

                Candidates = new ReactiveProperty<ExecutableUnitViewModel[]>().AddTo(CompositeDisposable);
                App.Instance.ObserveProperty(x => x.Candidates)
                    .ObserveOn(ThreadPoolScheduler.Instance)
                    .Subscribe(c =>
                    {
                        var old = Candidates.Value;

                        Candidates.Value =
                            c.Select(u => new ExecutableUnitViewModel(this, u)).ToArray();

                        if (old == null)
                            return;

                        foreach (var o in old)
                            o.Dispose();
                    })
                    .AddTo(CompositeDisposable);

                SelectedCandidate = Candidates
                    .Select(c => c?.FirstOrDefault())
                    .ToReactiveProperty()
                    .AddTo(CompositeDisposable);

                SelectedCandidateMoveCommand = SelectedCandidate
                    .Select(c => c != null)
                    .ToReactiveCommand()
                    .AddTo(CompositeDisposable);

                SelectedCandidateMoveCommand
                    .Subscribe(p =>
                    {
                        var current = IndexOfCandidates(SelectedCandidate.Value.Path);
                        var next = current + int.Parse((string) p);

                        if (next == -1)
                            next = Candidates.Value.Length - 1;
                        else if (next == Candidates.Value.Length)
                            next = 0;

                        SelectedCandidate.Value = Candidates.Value[next];
                    }).AddTo(CompositeDisposable);

                RunCommand = SelectedCandidate
                    .Select(i => i != null)
                    .ToReactiveCommand<string>().AddTo(CompositeDisposable);
                string path = null;
                RunCommand
                    .Do(_ =>
                    {
                        path = SelectedCandidate.Value.Path;
                        Input.Value = string.Empty;
                    })
                    .Delay(TimeSpan.FromMilliseconds(20))
                    .ObserveOnUIDispatcher()
                    .Subscribe(async p =>
                    {
                        var i = await App.Instance.RunAsync(path, p == "admin");
                        if (i)
                        {
                            Messenger.Publish(new WindowActionMessage(WindowAction.Hidden));
                            return;
                        }

                        var errMes = Properties.Resources.Message_FailedToStart;
                        if (File.Exists(path) == false)
                            errMes += Properties.Resources.Message_FileNotFound;

                        var item = new StatusBarItemViewModel(errMes);

                        StatusBar.Messages.AddOnScheduler(item);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        StatusBar.Messages.RemoveOnScheduler(item);
                        item.Dispose();
                    }).AddTo(CompositeDisposable);

                ContainingFolderOpenCommand = SelectedCandidate
                    .Select(i => i != null)
                    .ToReactiveCommand().AddTo(CompositeDisposable);
                ContainingFolderOpenCommand
                    .Subscribe(async _ => await OpenByExplorer(SelectedCandidate.Value.Path))
                    .AddTo(CompositeDisposable);

                SettingShowCommand = new AsyncReactiveCommand().AddTo(CompositeDisposable);
                SettingShowCommand.Subscribe(async _ =>
                {
                    using (Disposable.Create(() =>
                    {
                        App.Instance.SaveConfig();

                        if (VersionUpdater.Instance.IsRestartRequested)
                        {
                            ExitCommand.Execute(null);
                            return;
                        }

                        IsShowingSettingShow.Value = false;
                        Messenger.Publish(new WindowActionMessage(WindowAction.VisibleActive));
                        App.Instance.InvokeShortcutKeyChanged();
                    }))
                    {
                        var key = App.Instance.Config.ShortcutKeys.Activate.Key;
                        App.Instance.Config.ShortcutKeys.Activate.Key = Key.None;
                        App.Instance.InvokeShortcutKeyChanged();
                        App.Instance.Config.ShortcutKeys.Activate.Key = key;

                        IsShowingSettingShow.Value = true;
                        await AsyncMessageBroker.Default.PublishAsync(new SettingViewModel(App.Instance.Config));
                    }
                }).AddTo(CompositeDisposable);

                IsShowingSettingShow = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

                IsShowingSettingShow
                    .Subscribe(i => App.Instance.IsEnableAutoUpdater = i == false)
                    .AddTo(CompositeDisposable);

                ShowCommand = IsShowingSettingShow.Inverse().ToReactiveCommand().AddTo(CompositeDisposable);
                ShowCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.VisibleActive)))
                    .AddTo(CompositeDisposable);

                HideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
                HideCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.Hidden)))
                    .AddTo(CompositeDisposable);

                ExitCommand = IsShowingSettingShow.Inverse().ToReactiveCommand().AddTo(CompositeDisposable);
                ExitCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.Close)))
                    .AddTo(CompositeDisposable);

                IsEnableActivateHotKey = new ReactiveProperty<bool>(true).AddTo(CompositeDisposable);

                IndexOpeningResult = App.Instance.ObserveProperty(x => x.IndexOpeningResult)
                    .ToReadOnlyReactiveProperty()
                    .AddTo(CompositeDisposable);

                IndexOpeningResult
                    .Where(r => r == IndexOpeningResults.Ok)
                    .Subscribe(_ => Input.ForceNotify())
                    .AddTo(CompositeDisposable);

                StatusBar = new StatusBarViewModel(this).AddTo(CompositeDisposable);
            }
        }

        private async Task InitializeAsync()
        {
            Observable
                .Merge(Left)
                .Merge(Top)
                .Throttle(TimeSpan.FromSeconds(2))
                .Subscribe(_ => SaveConfig())
                .AddTo(CompositeDisposable);

            App.Instance.Config.ObserveProperty(x => x.IconCacheSize)
                .Subscribe(x => _iconDecoder.IconCacheSize = x)
                .AddTo(CompositeDisposable);

            CompositeDisposable.Add(DisposeCandidates);

            await App.Instance.OpenIndexAsync();

            if ((IndexOpeningResult.Value == IndexOpeningResults.NotFound) ||
                (IndexOpeningResult.Value == IndexOpeningResults.OldIndex))
                await UpdateIndexAsync();
        }

        private async Task UpdateIndexAsync()
        {
            using (Disposable.Create(() => IsIndexUpdating.Value = false))
            {
                IsIndexUpdating.Value = true;
                await App.Instance.UpdateIndexAsync();
                Input.ForceNotify();
            }
        }

        private void DisposeCandidates()
        {
            var candidates = Candidates?.Value;

            if (candidates == null)
                return;

            foreach (var c in candidates)
                c.Dispose();
        }

        private int IndexOfCandidates(string path)
        {
            var index = 0;
            foreach (var c in Candidates.Value)
            {
                if (c.Path == path)
                    return index;

                ++ index;
            }

            return -1;
        }

        private void LoadConfig()
        {
            Debug.Assert(Config == null);

            Config = ConfigHelper.ReadConfig<Core.Config.MainWindow>(ConfigHelper.Category.MainWindow,
                Constants.ConfigDirPath);
        }

        private void SaveConfig()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.MainWindow, Constants.ConfigDirPath, Config);
        }

        private static async Task OpenByExplorer(string path)
        {
            await ProcessHelper.RunAsync("EXPLORER", $"/select,\"{path}\"", false);
        }
    }
}