using System;
using System.Diagnostics;
using System.Threading;

namespace Ann.Foundation
{
    public class AnonymousDisposable : IDisposable
    {
        private readonly Action _dispose;
        private int _isDisposed;
 
        public AnonymousDisposable(Action dispose)
        {
            Debug.Assert(dispose != null);

            _dispose = dispose;
        }
 
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _dispose();
            GC.SuppressFinalize(this);
        }
    }
}