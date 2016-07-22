using System.Diagnostics;
using System.Linq;
using Ann.Foundation.Mvvm;
using Ann.SettingWindow.SettingPage.General;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow
{
    public class SettingViewModel : ViewModel
    {
        public ViewModelBase[] Pages { get; set; }
        public ReactiveProperty<ViewModelBase> SelectedPage { get; }

        public SettingViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            CompositeDisposable.Add(() => Pages.ForEach(p => p.Dispose()));

            Pages = new ViewModelBase[]
            {
                new GeneralViewModel(model),
                new ShortcutsViewModel(model),
                new TargetFoldersViewModel(model),
                new PriorityFilesViewModel(model)
            };
            SelectedPage = new ReactiveProperty<ViewModelBase>(Pages[0]).AddTo(CompositeDisposable);
        }
    }
}