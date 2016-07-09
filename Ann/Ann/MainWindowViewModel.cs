using System;
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

        private ExecutableUnitHolder _holder;

        public MainWindowViewModel()
        {
            CompositeDisposable.Add(Input);
            CompositeDisposable.Add(Message);
            CompositeDisposable.Add(UpdateCommand);
            CompositeDisposable.Add(DisposeHolder);

            UpdateHolder();

            UpdateCommand.Subscribe(async _ =>
            {
                Message.Value = "Updating";

                DisposeHolder();
                await UpdateIndexAsync();
                UpdateHolder();

                Message.Value = string.Empty;
            }).AddTo(CompositeDisposable);

            Candidates = Input
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(input => _holder.Find(input).ToArray())
                .ToReadOnlyReactiveProperty()
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