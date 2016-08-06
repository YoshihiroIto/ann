using Ann.Core.Config;
using Xunit;

namespace Ann.Core.Test.Config
{
    public class MainWindowTest
    {
        [Fact]
        public void Basic()
        {
            var c = new MainWindow();

            Assert.True(double.IsNaN(c.Left));
            Assert.True(double.IsNaN(c.Top));

            c.Left = 123;
            c.Top = 456;

            Assert.Equal(123, c.Left);
            Assert.Equal(456, c.Top);
        }
    }
}