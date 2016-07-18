using System.Diagnostics;
using Ann.Foundation.Mvvm;

namespace Ann.SettingWindow.SettingPage
{
    public class ShortcutsViewModel : ViewModelBase
    {
        public string Name { get; } = "Shortcuts";

        public ShortcutKeyViewModel Activate { get; set; }
        public ShortcutKeyListBoxViewModel HideShortcuts { get; }

        public ShortcutsViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            Activate = new ShortcutKeyViewModel(model.MainWindow.ShortcutKeys.Activate);
            HideShortcuts = new ShortcutKeyListBoxViewModel(model.MainWindow.ShortcutKeys.Hide);
        }
    }
}