using System;
using System.Windows.Input;
using System.Windows.Interop;
using Ann.Foundation.Control;
using Xunit;

namespace Ann.Foundation.Test.Control
{
    public class ShortcutKeyInputBoxTest
    {
        [WpfFact]
        public void Basic()
        {
            var c = new ShortcutKeyInputBox();

            Assert.Equal(Key.None, c.Key);
            c.Key = Key.A;
            Assert.Equal(Key.A, c.Key);
            c.Key = Key.None;
            Assert.Equal(Key.None, c.Key);

            Assert.False(c.IsControl);
            c.IsControl = true;
            Assert.True(c.IsControl);

            Assert.False(c.IsAlt);
            c.IsAlt = true;
            Assert.True(c.IsAlt);

            Assert.False(c.IsShift);
            c.IsShift = true;
            Assert.True(c.IsShift);

            c.Key = Key.Z;
            Assert.Equal("Ctrl + Alt + Shift + Z", c.Text);
        }

        [WpfFact]
        public void KeyInput()
        {
            var c = new ShortcutKeyInputBox();

            c.RaiseEvent(
                new KeyEventArgs(
                    new MockKeyboardDevice(),
                    new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero),
                    0,
                    Key.B) {RoutedEvent = Keyboard.PreviewKeyDownEvent});

            Assert.Equal("B", c.Text);

            c.RaiseEvent(
                new KeyEventArgs(
                    new MockKeyboardDevice { Modifiers = ModifierKeys.Control},
                    new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero),
                    0,
                    Key.Space) {RoutedEvent = Keyboard.PreviewKeyDownEvent});

            Assert.Equal("Ctrl + Space", c.Text);

            c.RaiseEvent(
                new KeyEventArgs(
                    new MockKeyboardDevice(),
                    new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero),
                    0,
                    Key.LeftCtrl) {RoutedEvent = Keyboard.PreviewKeyDownEvent});

            Assert.Equal(string.Empty, c.Text);

            c.RaiseEvent(
                new KeyEventArgs(
                    new MockKeyboardDevice {Modifiers = ModifierKeys.Control | ModifierKeys.Alt},
                    new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero),
                    0,
                    Key.X) {RoutedEvent = Keyboard.PreviewKeyDownEvent});

            Assert.Equal("Ctrl + Alt + X", c.Text);
        }
    }
}