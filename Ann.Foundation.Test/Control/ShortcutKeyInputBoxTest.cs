using System.Windows.Input;
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
    }
}