using System;
using System.Windows.Threading;
using Xunit;

namespace Ann.Test
{
    public class ViewManagerTest :  IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            using (_context.GetInstance<ViewManager>())
            {
            }
        }
    }
}