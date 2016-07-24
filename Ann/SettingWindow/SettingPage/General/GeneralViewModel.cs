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
        public ReactiveProperty<SelectableCulture> SelectedCulture { get; }

        public static readonly int[] MaxCandidateLines
            = Enumerable.Range(1, 10).ToArray();

        public static readonly SelectableCulture[] SelectableCulture =
        {
            new SelectableCulture {Caption = "<Auto>", CultureName = string.Empty},
            new SelectableCulture {Caption = "English", CultureName = "en-US"},
            new SelectableCulture {Caption = "日本語", CultureName = "ja-JP"}
        };

        public GeneralViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            MaxCandidateLinesCount = model.MainWindow.ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                .AddTo(CompositeDisposable);

            SelectedCulture =
                model.ToReactivePropertyAsSynchronized(
                    x => x.Culture,
                    convert: x => SelectableCulture.FirstOrDefault(y => y.CultureName == x) ?? SelectableCulture[0],
                    convertBack: x => x.CultureName
                    ).AddTo(CompositeDisposable);
        }
    }

    public class SelectableCulture
    {
        public string Caption { get; set; }
        public string CultureName { get; set; }
    }
}