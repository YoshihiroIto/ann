using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Livet.Messaging.Windows;

namespace Ann
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<bool> CanIndexUpdate { get; } = new ReactiveProperty<bool>(true);
        public ReactiveCommand IndexUpdateCommand { get; }

        public ReadOnlyReactiveProperty<ExecutableUnitViewModel[]> Candidates { get; }
        public ReactiveProperty<ExecutableUnitViewModel> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand RunCommand { get; }

        public ReactiveProperty<Visibility> Visibility { get; }
            = new ReactiveProperty<Visibility>(System.Windows.Visibility.Visible);

        public ReactiveCommand AppHideCommand { get; }
        public ReactiveCommand AppQuitCommand { get; }

        public ReactiveProperty<double> Left { get;  }
        public ReactiveProperty<double> Top { get;  }

        private ExecutableUnitHolder _holder;

        public MainWindowViewModel()
        {
            CompositeDisposable.Add(Input);
            CompositeDisposable.Add(Message);
            CompositeDisposable.Add(CanIndexUpdate);
            CompositeDisposable.Add(Visibility);
            CompositeDisposable.Add(DisposeHolder);

            UpdateHolder();

            IndexUpdateCommand = CanIndexUpdate
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            IndexUpdateCommand
                .Subscribe(async _ =>
                {
                    CanIndexUpdate.Value = false;
                    Message.Value = "Updating";

                    Input.Value = string.Empty;

                    DisposeHolder();
                    await UpdateIndexAsync();
                    UpdateHolder();
                    Input.ForceNotify();

                    Message.Value = string.Empty;
                    CanIndexUpdate.Value = true;
                }).AddTo(CompositeDisposable);

            Candidates = Input
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(i =>
                {
                    return _holder?
                        .Find(i)
                        .OrderByDescending(u => App.Instance.IsHighPriority(u.Path))
                        .Select(u => new ExecutableUnitViewModel(u))
                        .ToArray();
                })
                .ToReadOnlyReactiveProperty()
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
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            RunCommand
                .Subscribe(async _ =>
                {
                    Visibility.Value = System.Windows.Visibility.Hidden;
                    await Task.Run(() => Process.Start(SelectedCandidate.Value.Path));
                }).AddTo(CompositeDisposable);

            AppHideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppHideCommand.Subscribe(_ => Visibility.Value = System.Windows.Visibility.Hidden)
                .AddTo(CompositeDisposable);

            AppQuitCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            AppQuitCommand.Subscribe(_ => Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close")))
                .AddTo(CompositeDisposable);

            Left = App.Instance.ToReactivePropertyAsSynchronized(x => x.MainWindowLeft).AddTo(CompositeDisposable);
            Top = App.Instance.ToReactivePropertyAsSynchronized(x => x.MainWindowTop).AddTo(CompositeDisposable);
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


        private void UpdateHolder()
        {
            _holder = new ExecutableUnitHolder("Index.db");
        }

        private void DisposeHolder()
        {
            _holder?.Dispose();
            _holder = null;
        }

        private static async Task UpdateIndexAsync()
        {
            await Crawler.ExecuteAsync(
                "Index.db",
                new[]
                {
                    @"C:\Program Files",
                    @"C:\Program Files (x86)"
                });
        }
    }
}