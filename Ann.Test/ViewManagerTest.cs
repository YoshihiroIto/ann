using System;
using System.Windows.Threading;
using Ann.Core;
using Xunit;

namespace Ann.Test
{
    public class ViewManagerTest :  IDisposable
    {
        public ViewManagerTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            using (new ViewManager(Dispatcher.CurrentDispatcher))
            {
            }
        }
    }
}