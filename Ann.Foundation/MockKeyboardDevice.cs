using System.Windows.Input;

namespace Ann.Foundation
{
    public class MockKeyboardDevice : KeyboardDevice
    {
        public new ModifierKeys Modifiers { get; set; }

        public MockKeyboardDevice()
            : base(InputManager.Current)
        {
        }

        protected override KeyStates GetKeyStatesFromSystem(Key key)
        {
            switch (key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    return (Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt ? KeyStates.Down : KeyStates.None;
                    
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return (Modifiers & ModifierKeys.Control) == ModifierKeys.Control ? KeyStates.Down : KeyStates.None;

                case Key.LeftShift:
                case Key.RightShift:
                    return (Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? KeyStates.Down : KeyStates.None;
            }

            return KeyStates.None;
        }
    }
}
