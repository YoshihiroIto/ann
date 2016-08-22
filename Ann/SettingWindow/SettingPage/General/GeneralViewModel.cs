using System;
using System.Diagnostics;
using System.Linq;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.General
{
    public class GeneralViewModel : ViewModelBase
    {
        public ReactiveProperty<int> MaxCandidateLinesCount { get; }
        public ReactiveProperty<CultureSummry> SelectedCulture { get; }
        public ReactiveProperty<bool> IsStartOnSystemStartup { get; }

        public ReactiveCommand HelpTranslateOpenCommand { get; }

        public static readonly int[] MaxCandidateLines
            = Enumerable.Range(1, 10).ToArray();

        public GeneralViewModel(Core.Config.App model, VersionUpdater versionUpdater)
        {
            Debug.Assert(model != null);

            MaxCandidateLinesCount = model.ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                .AddTo(CompositeDisposable);

            IsStartOnSystemStartup = model.ToReactivePropertyAsSynchronized(x => x.IsStartOnSystemStartup)
                .AddTo(CompositeDisposable);

            IsStartOnSystemStartup
                .Subscribe(async i =>
                {
                    if (i)
                        await versionUpdater.CreateStartupShortcut();
                    else
                        await versionUpdater.RemoveStartupShortcut();
                }).AddTo(CompositeDisposable);

            SelectedCulture =
                model.ToReactivePropertyAsSynchronized(
                    x => x.Culture,
                    convert: x => Constants.SupportedCultures.FirstOrDefault(y => y.CultureName == x) ?? Constants.SupportedCultures[0],
                    convertBack: x => x.CultureName
                    ).AddTo(CompositeDisposable);

            HelpTranslateOpenCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            HelpTranslateOpenCommand
                .Subscribe(async _ => await ProcessHelper.RunAsync(Constants.AnnLocalizationUrl, string.Empty, false))
                .AddTo(CompositeDisposable);
        }
    }

    public class SelectableCulture
    {
        public string Caption { get; set; }
        public string CultureName { get; set; }
    }
}