using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using System.Linq;

namespace Ann.SettingWindow.SettingPage
{
    public class GeneralViewModel : ViewModelBase
    {
        public string Name { get; } = "General";

        public ReactiveProperty<int> MaxCandidateLinesCount { get; }

        public static  int[] MaxCandidateLines { get; }
            = Enumerable.Range(2, 9).ToArray();

        public GeneralViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            MaxCandidateLinesCount = new ReactiveProperty<int>(model.MainWindow.MaxCandidateLinesCount);
        }
    }
}