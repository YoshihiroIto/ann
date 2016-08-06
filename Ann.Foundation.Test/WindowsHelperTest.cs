using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowsHelperTest
    {
        [WpfFact]
        public void IsOnTrayMouseCursor()
        {
            Assert.False(WindowsHelper.IsOnTrayMouseCursor);
        }
    }
}
