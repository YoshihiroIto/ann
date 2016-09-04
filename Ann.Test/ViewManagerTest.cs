using System;
using Ann.Core;
using Xunit;

namespace Ann.Test
{
    public class ViewManagerTest :  IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
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