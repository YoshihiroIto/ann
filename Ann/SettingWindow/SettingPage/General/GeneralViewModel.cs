using System.Diagnostics;
using System.Linq;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.General
{
    public class GeneralViewModel : ViewModelBase
    {
        public string Name { get; } = "General";
        public bool IsAbout { get; } = false;

        public ReactiveProperty<int> MaxCandidateLinesCount { get; }

        public static int[] MaxCandidateLines { get; }
            = Enumerable.Range(1, 10).ToArray();

        public GeneralViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            MaxCandidateLinesCount = model.MainWindow.ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                .AddTo(CompositeDisposable);
        }
    }
}