using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ann.Core;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class MainWindowViewModel : ViewModel
    {
        public ReactiveProperty<string> Input { get; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<bool> CanIndexUpdate { get; } = new ReactiveProperty<bool>(true);
        public ReactiveCommand IndexUpdateCommand { get; }

        public ReadOnlyReactiveProperty<ExecutableUnit[]> Candidates { get; }
        public ReactiveProperty<ExecutableUnit> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand RunCommand { get; }

        public ReactiveProperty<Visibility> Visibility { get; }
            = new ReactiveProperty<Visibility>(System.Windows.Visibility.Visible);
        public ReactiveCommand HideCommand { get; }
 
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
                .Select(i => _holder?.Find(i).ToArray())
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
                    var current = Array.IndexOf(Candidates.Value, SelectedCandidate.Value);
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
                .Subscribe(_ => Process.Start(SelectedCandidate.Value.Path))
                .AddTo(CompositeDisposable);

            HideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            HideCommand.Subscribe(_ => Visibility.Value = System.Windows.Visibility.Hidden)
                .AddTo(CompositeDisposable);
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
            var sw = new Stopwatch();

            sw.Start();

            await Crawler.ExecuteAsync(
                "Index.db",
                new[]
                {
                    @"C:\Program Files",
                    @"C:\Program Files (x86)"
                });

            sw.Stop();

            Debug.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}