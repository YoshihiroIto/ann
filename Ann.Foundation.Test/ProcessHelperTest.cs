using System;
using System.Windows.Threading;
using Xunit;

namespace Ann.Foundation.Test
{
    public class ProcessHelperTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private static string app = @"..\..\..\Ann.GenOpenSourceList\Ann.GenOpenSourceList.exe";

        [Fact]
        public void Basic()
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
