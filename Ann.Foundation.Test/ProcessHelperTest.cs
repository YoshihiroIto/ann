using Xunit;

namespace Ann.Foundation.Test
{
    public class ProcessHelperTest
    {
        private static string app = @"..\..\..\Ann.GenOpenSourceList\Ann.GenOpenSourceList.exe";

        [Fact]
        public void Simple()
        {
            ProcessHelper.RunAsync(app, "", false).Wait();
        }

        [Fact]
        public void IgnoreError()
        {
            ProcessHelper.RunAsync("XXXXX", "", false).Wait();
        }

        [Fact]
        public void RunAsAdmin()
        {
            ProcessHelper.RunAsync(app, "", true).Wait();
        }
    }
}
