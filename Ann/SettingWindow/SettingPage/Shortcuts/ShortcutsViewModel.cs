using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutsViewModel : ViewModelBase
    {
        public ShortcutKeyViewModel Activate { get; set; }
        public ShortcutKeyListBoxViewModel HideShortcuts { get; }

        public ShortcutsViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            Activate = new ShortcutKeyViewModel(model.ShortcutKeys.Activate)
                .AddTo(CompositeDisposable);

            HideShortcuts = new ShortcutKeyListBoxViewModel(model.ShortcutKeys.Hide)
                .AddTo(CompositeDisposable);
        }
    }
}