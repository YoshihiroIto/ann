using System;
using System.Diagnostics;
using System.Threading;
using Ann.Foundation.Exceptions;

namespace Ann.Foundation
{
    public class AnonymousDisposable : IDisposable
    {
        private readonly Action _dispose;
        private int _isDisposed;

        public AnonymousDisposable() : this(() => { })
        {
        }
 
        public AnonymousDisposable(Action dispose)
        {
            Debug.Assert(dispose != null);

            _dispose = dispose;
        }
 
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                throw new MultipleDisposingException();

            _dispose();
            GC.SuppressFinalize(this);
        }
    }
}