using System.Windows.Input;
using Ann.Foundation.Mvvm;

namespace Ann.Config
{
    public class ShortcutKey : ModelBase
    {
        #region Key

        private Key _Key;

        public Key Key
        {
            get { return _Key; }
            set { SetProperty(ref _Key, value); }
        }

        #endregion

        #region Modifiers

        private ModifierKeys _Modifiers;

        public ModifierKeys Modifiers
        {
            get { return _Modifiers; }
            set { SetProperty(ref _Modifiers, value); }
        }

        #endregion
    }
}