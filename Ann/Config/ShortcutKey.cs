using System.Windows.Input;

namespace Ann.Config
{
    public class ShortcutKey
    {
        public Key Key { set; get; }
        public ModifierKeys Modifiers { get; set; }
    }
}