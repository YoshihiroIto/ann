using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.SettingWindow;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; }
        public ReactiveProperty<string> InProgressMessage { get; }
        public ReadOnlyReactiveProperty<bool> InProgress { get; }
        public ReactiveCommand IndexUpdateCommand { get; }

        public ReadOnlyReactiveProperty<ObservableCollection<ExecutableUnitViewModel>> Candidates { get; }
        public ReactiveProperty<ExecutableUnitViewModel> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand<string> RunCommand { get; }
        public ReactiveCommand ContainingFolderOpenCommand { get; }

        public ReactiveProperty<Visibility> Visibility { get; }

        public ReactiveCommand AppHideCommand { get; }
        public ReactiveCommand AppExitCommand { get; }

        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<int> MaxCandidatesLinesCount { get; }

        private readonly IconDecoder _iconDecoder = new IconDecoder();

        public ImageSource GetIcon(string path) => _iconDecoder.GetIcon(path);
        public ReadOnlyReactiveProperty<double> CandidatesListMaxHeight { get; }
        public ReactiveProperty<double> CandidateItemHeight { get; }

        public ReactiveCommand SettingShowCommand { get; }

        public ReadOnlyReactiveProperty<IndexOpeningResults> IndexOpeningResult { get; }
        public ReactiveProperty<bool> IsEnableActivateHotKey { get; }
        public ReadOnlyReactiveProperty<string> Message { get; }

        public MainWindowViewModel()
        {
            Input = new ReactiveProperty<string>().AddTo(CompositeDisposable);
            InProgressMessage = new ReactiveProperty<string>(string.Empty).AddTo(CompositeDisposable);
            Visibility = new ReactiveProperty<Visibility>(System.Windows.Visibility.Visible).AddTo(CompositeDisposable);

            CompositeDisposable.Add(DisposeCandidates);

            InProgress = InProgressMessage
                .Select(m => string.IsNullOrEmpty(m) == false)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            Left =
                App.Instance.Config.MainWindow.ToReactivePropertyAsSynchronized(x => x.Left).AddTo(CompositeDisposable);
            Top = App.Instance.Config.MainWindow.ToReactivePropertyAsSynchronized(x => x.Top).AddTo(CompositeDisposable);
            MaxCandidatesLinesCount =
                App.Instance.Config.MainWindow.ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                    .AddTo(CompositeDisposable);

            CandidateItemHeight = new ReactiveProperty<double>().AddTo(CompositeDisposable);
            CandidatesListMaxHeight =
                Observable
                    .Merge(MaxCandidatesLinesCount.ToUnit())
                    .Merge(CandidateItemHeight.ToUnit())
                    .Select(_ => CandidateItemHeight.Value*MaxCandidatesLinesCount.Value + 4)
                    .ToReadOnlyReactiveProperty()
                    .AddTo(CompositeDisposable);

            IndexUpdateCommand = InProgressMessage.Select(string.IsNullOrEmpty)
                .ToReactiveCommand().AddTo(CompositeDisposable);

            IndexUpdateCommand
                .Subscribe(async _ =>
                {
                    using (new AnonymousDisposable(() => InProgressMessage.Value = string.Empty))
                    {
                        InProgressMessage.Value = "Index Updating...";

                        await App.Instance.UpdateIndexAsync();
                        Input.ForceNotify();
                    }
                }).AddTo(CompositeDisposable);

            Candidates = Input
                .Select(i =>
                {
                    DisposeCandidates();

                    return
                        new ObservableCollection<ExecutableUnitViewModel>(
                            App.Instance.FindExecutableUnit(i)
                                .Select(u => new ExecutableUnitViewModel(this, u)));
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            SelectedCandidate = Candidates
                .Select(c => c?.FirstOrDefault())
                .ToReactiveProperty()
                .AddTo(CompositeDisposable);

            SelectedCandidateMoveCommand = SelectedCandidate
                .Select(c => c != null)
                .ToReactiveCommand().AddTo(CompositeDisposable);
            SelectedCandidateMoveCommand
                .Subscribe(p =>
                {
                    var current = IndexOfCandidates(SelectedCandidate.Value.Path);
                    var next = current + int.Parse((string) p);

                    if (next == -1)
                        next = Candidates.Value.Count - 1;
                    else if (next == Candidates.Value.Count)
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
                .Subscribe(async p =>
                {
                    Visibility.Value = System.Windows.Visibility.Hidden;
                    await ProcessHelper.Run(path, string.Empty, p == "admin");
                }).AddTo(CompositeDisposable);

            ContainingFolderOpenCommand = SelectedCandidate
                .Select(i => i != null)
                .ToReactiveCommand().AddTo(CompositeDisposable);
            ContainingFolderOpenCommand
                .Subscribe(
                    async _ => await ProcessHelper.Run("EXPLORER", $"/select,\"{SelectedCandidate.Value.Path}\"", false))
                .AddTo(CompositeDisposable);

            AppHideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppHideCommand.Subscribe(_ => Visibility.Value = System.Windows.Visibility.Hidden)
                .AddTo(CompositeDisposable);

            AppExitCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppExitCommand.Subscribe(_ => Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction")))
                .AddTo(CompositeDisposable);

            Visibility.Subscribe(_ => Input.Value = string.Empty).AddTo(CompositeDisposable);

            SettingShowCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            SettingShowCommand.Subscribe(_ =>
            {
                // 一旦止める
                var key = App.Instance.Config.MainWindow.ShortcutKeys.Activate.Key;
                App.Instance.Config.MainWindow.ShortcutKeys.Activate.Key = Key.None;
                App.Instance.InvokeShortcutKeyChanged();
                App.Instance.Config.MainWindow.ShortcutKeys.Activate.Key = key;

                Messenger.Raise(new TransitionMessage(new SettingViewModel(App.Instance.Config), "ShowSetting"));

                App.Instance.SaveConfig();
                App.Instance.InvokeShortcutKeyChanged();
            }).AddTo(CompositeDisposable);

            IndexOpeningResult = App.Instance.ObserveProperty(x => x.IndexOpeningResult)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            IsEnableActivateHotKey = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            Message = IsEnableActivateHotKey
                .Select(i => i ? string.Empty : "Activation Hotkey is already in use.")
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }

        private void DisposeCandidates()
        {
            var candidates = Candidates?.Value;
            if (candidates != null)
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
    }
}