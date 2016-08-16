using System;
using System.Windows.Threading;
using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowsHelperTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void IsOnTrayMouseCursor()
        {
            Assert.False(WindowsHelper.IsOnTrayMouseCursor);
        }
    }
}
