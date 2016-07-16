using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.SettingWindow;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; }
        public ReactiveProperty<bool> IsIndexUpdating { get; }
        public ReactiveCommand IndexUpdateCommand { get; }

        public ReadOnlyReactiveProperty<ObservableCollection<ExecutableUnitViewModel>> Candidates { get; }
        public ReactiveProperty<ExecutableUnitViewModel> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand RunCommand { get; }

        public ReactiveProperty<Visibility> Visibility { get; }

        public ReactiveCommand AppHideCommand { get; }
        public ReactiveCommand AppExitCommand { get; }

        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<int> MaxCandidatesLinesCount { get; }

        private readonly IconDecoder _iconDecoder = new IconDecoder(App.IconCacheFilePath);

        public Size IconSize
        {
            set { _iconDecoder.IconSize = value; }
        }

        public ImageSource GetIcon(string path) => _iconDecoder.GetIcon(path);
        public ReadOnlyReactiveProperty<double> CandidatesListMaxHeight { get; }
        public ReactiveProperty<double> CandidateItemHeight { get; }

        public ReactiveCommand SettingShowCommand { get; }

        public ReadOnlyReactiveProperty<bool> IsEnabledIndex { get; }
        public ReactiveProperty<bool> IsEnableActivateHotKey { get; }

        public ReadOnlyReactiveProperty<string> Message { get; }

        public MainWindowViewModel()
        {
            Input = new ReactiveProperty<string>().AddTo(CompositeDisposable);
            IsIndexUpdating = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            Visibility = new ReactiveProperty<Visibility>(System.Windows.Visibility.Visible).AddTo(CompositeDisposable);

            Left = App.Instance.ToReactivePropertyAsSynchronized(x => x.MainWindowLeft).AddTo(CompositeDisposable);
            Top = App.Instance.ToReactivePropertyAsSynchronized(x => x.MainWindowTop).AddTo(CompositeDisposable);
            MaxCandidatesLinesCount =
                App.Instance.ToReactivePropertyAsSynchronized(x => x.MainWindowMaxCandidateLinesCount)
                    .AddTo(CompositeDisposable);

            CandidateItemHeight = new ReactiveProperty<double>().AddTo(CompositeDisposable);
            CandidatesListMaxHeight =
                CandidateItemHeight
                    .Select(h => h*MaxCandidatesLinesCount.Value + 4)
                    .ToReadOnlyReactiveProperty()
                    .AddTo(CompositeDisposable);

            IndexUpdateCommand = IsIndexUpdating.Select(i => i == false)
                .ToReactiveCommand().AddTo(CompositeDisposable);

            IndexUpdateCommand
                .Subscribe(async _ =>
                {
                    IsIndexUpdating.Value = true;

                    await App.Instance.UpdateIndexAsync();
                    Input.ForceNotify();

                    IsIndexUpdating.Value = false;
                }).AddTo(CompositeDisposable);

            Candidates = Input
                .Throttle(TimeSpan.FromMilliseconds(20))
                .Select(i =>
                {
                    var candidates = Candidates?.Value;
                    if (candidates != null)
                        foreach (var c in candidates)
                            c.Dispose();

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
                .ToReactiveCommand().AddTo(CompositeDisposable);
            string path = null;
            RunCommand
                .Do(_ =>
                {
                    path = SelectedCandidate.Value.Path;
                    Input.Value = string.Empty;
                })
                .Delay(TimeSpan.FromMilliseconds(20))
                .Subscribe(_ =>
                {
                    Visibility.Value = System.Windows.Visibility.Hidden;
                    Process.Start(path);
                }).AddTo(CompositeDisposable);

            AppHideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppHideCommand.Subscribe(_ => Visibility.Value = System.Windows.Visibility.Hidden)
                .AddTo(CompositeDisposable);

            AppExitCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppExitCommand.Subscribe(_ => Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction")))
                .AddTo(CompositeDisposable);

            Visibility.Subscribe(_ => Input.Value = string.Empty).AddTo(CompositeDisposable);

            SettingShowCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            SettingShowCommand.Subscribe(_ =>
                Messenger.Raise(new TransitionMessage(
                    new SettingViewModel(App.Instance.MakeCurrentConfig()),
                    "ShowSetting"))
                ).AddTo(CompositeDisposable);

            IsEnabledIndex = App.Instance.ObserveProperty(x => x.IsEnabledIndex)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            IsEnableActivateHotKey = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            Message = IsEnableActivateHotKey
                .Select(i => i ? string.Empty : "Activation Hotkey is already in use.")
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
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