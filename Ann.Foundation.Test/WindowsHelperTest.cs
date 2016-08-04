using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowsHelperTest
    {
        [StaFact]
        public void IsOnTrayMouseCursor()
        {
            Assert.False(WindowsHelper.IsOnTrayMouseCursor);
        }
    }
}
