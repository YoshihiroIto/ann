using System.Diagnostics;
using System.Linq;
using System.Windows;
using Ann.Foundation.Mvvm;
using Ann.SettingWindow.SettingPage;
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
                new TargetFoldersViewModel(model),
                new ShortcutsViewModel(model)
            };
            SelectedPage = new ReactiveProperty<ViewModelBase>(Pages[0]).AddTo(CompositeDisposable);
        }
    }
}