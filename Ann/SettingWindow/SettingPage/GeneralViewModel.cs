﻿using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using System.Linq;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage
{
    public class GeneralViewModel : ViewModelBase
    {
        public string Name { get; } = "General";

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