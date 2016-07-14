using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Livet;
using Reactive.Bindings;

namespace Ann.SettingWindow
{
    public class SettingViewModel : ViewModel
    {
        public ReactiveProperty<int> MaxCandidateLinesCount { get; }
        public ObservableCollection<string> TargetFolders { get; }
        public ObservableCollection<string> HighPriorities { get; }

        public IList<int> MaxCandidateLines { get; }

        public SettingViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            MaxCandidateLines = Enumerable.Range(2, 9).ToArray();

            MaxCandidateLinesCount = new ReactiveProperty<int>(model.MainWindow.MaxCandidateLinesCount);
            TargetFolders = new ObservableCollection<string>(model.TargetFolders);
            HighPriorities = new ObservableCollection<string>(model.HighPriorities);
        }
    }
}