using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        public ReactiveCommand UpdateCommand { get; } = new ReactiveCommand();

        public ReadOnlyReactiveProperty<ExecutableUnit[]> Candidates { get; }
        public ReactiveProperty<ExecutableUnit> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand RunCommand { get; }

        private ExecutableUnitHolder _holder;

        public MainWindowViewModel()
        {
            CompositeDisposable.Add(Input);
            CompositeDisposable.Add(Message);
            CompositeDisposable.Add(UpdateCommand);
            CompositeDisposable.Add(DisposeHolder);

            UpdateHolder();

            UpdateCommand
                .Subscribe(async _ =>
                {
                    Message.Value = "Updating";

                    Input.Value = string.Empty;
                    DisposeHolder();
                    await UpdateIndexAsync();
                    UpdateHolder();

                    Message.Value = string.Empty;
                }).AddTo(CompositeDisposable);

            Candidates = Input
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(i => _holder.Find(i).ToArray())
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
        }

        private void UpdateHolder()
        {
            _holder = new ExecutableUnitHolder("Index.db");
        }

        private void DisposeHolder()
        {
            _holder?.Dispose();
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