using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ann.Foundation.Control
{
    public class ShortcutKeyInputBox : TextBox
    {
        #region Key

        public Key Key
        {
            get { return (Key) GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                nameof(Key),
                typeof(Key),
                typeof(ShortcutKeyInputBox),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnKeyChanged,
                    DefaultValue = default(Key),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ShortcutKeyInputBox)?.UpdateText();
        }

        #endregion

        #region IsControl

        public bool IsControl
        {
            get { return (bool) GetValue(IsControlProperty); }
            set { SetValue(IsControlProperty, value); }
        }

        public static readonly DependencyProperty IsControlProperty =
            DependencyProperty.Register(
                nameof(IsControl),
                typeof(bool),
                typeof(ShortcutKeyInputBox),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnIsControlChanged,
                    DefaultValue = default(bool),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnIsControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ShortcutKeyInputBox)?.UpdateText();
        }

        #endregion

        #region IsAlt

        public bool IsAlt
        {
            get { return (bool) GetValue(IsAltProperty); }
            set { SetValue(IsAltProperty, value); }
        }

        public static readonly DependencyProperty IsAltProperty =
            DependencyProperty.Register(
                nameof(IsAlt),
                typeof(bool),
                typeof(ShortcutKeyInputBox),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnIsAltChanged,
                    DefaultValue = default(bool),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnIsAltChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ShortcutKeyInputBox)?.UpdateText();
        }

        #endregion

        #region IsShift

        public bool IsShift
        {
            get { return (bool) GetValue(IsShiftProperty); }
            set { SetValue(IsShiftProperty, value); }
        }

        public static readonly DependencyProperty IsShiftProperty =
            DependencyProperty.Register(
                nameof(IsShift),
                typeof(bool),
                typeof(ShortcutKeyInputBox),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnIsShiftChanged,
                    DefaultValue = default(bool),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnIsShiftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ShortcutKeyInputBox)?.UpdateText();
        }

        #endregion

        #region IsFocused

        public new bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }

        public new static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                nameof (IsFocused),
                typeof (bool),
                typeof (ShortcutKeyInputBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(bool),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        public ShortcutKeyInputBox()
        {
            IsReadOnly = true;

            IsVisibleChanged += (_, __) =>
            {
                if (IsVisible == false)
                    IsFocused = false;
            };
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var modifierKeys = e.KeyboardDevice.Modifiers;

            var isControl = (modifierKeys & ModifierKeys.Control) != 0;
            var isAlt = (modifierKeys & ModifierKeys.Alt) != 0;
            var isShift = (modifierKeys & ModifierKeys.Shift) != 0;

            Key key;
            {
                if (isControl && isAlt)
                    key = e.Key;
                else
                    key = isAlt ? e.SystemKey : e.Key;

                if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                    key == Key.System || key == Key.LeftAlt || key == Key.RightAlt ||
                    key == Key.LeftShift || key == Key.RightShift)
                {
                    key = Key.None;
                    isControl = false;
                    isAlt = false;
                    isShift = false;
                }
            }

            Key = key;
            IsControl = isControl;
            IsAlt = isAlt;
            IsShift = isShift;
            
            e.Handled = true;
            base.OnPreviewKeyDown(e);
        }

        private void UpdateText()
        {
            if (Key == Key.None)
            {
                Text = string.Empty;
                return;
            }

            var sb = new StringBuilder();

            if (IsControl)
                sb.Append("Ctrl + ");
            if (IsAlt)
                sb.Append("Alt + ");
            if (IsShift)
                sb.Append("Shift + ");

            sb.Append(Key);

            Text = sb.ToString();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            IsFocused = true;

            base.OnGotFocus(e);

        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            IsFocused = false;

            base.OnLostFocus(e);
        }
    }
}