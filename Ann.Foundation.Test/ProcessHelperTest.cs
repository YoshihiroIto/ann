using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class ProcessHelperTest
    {
        private static string app = @"..\..\..\Ann.GenOpenSourceList\Ann.GenOpenSourceList.exe";

        [TestMethod]
        public void Simple()
        {
            ProcessHelper.RunAsync(app, "", false).Wait();
        }

        [TestMethod]
        public void IgnoreError()
        {
            ProcessHelper.RunAsync("XXXXX", "", false).Wait();
        }

        [TestMethod]
        public void RunAsAdmin()
        {
            ProcessHelper.RunAsync(app, "", true).Wait();
        }
    }
}
