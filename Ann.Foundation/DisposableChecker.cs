using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Ann.Foundation.Exception;

namespace Ann.Foundation
{
    public static class DisposableChecker
    {
        private static readonly ConcurrentDictionary<IDisposable, int> Disposables = new ConcurrentDictionary<IDisposable, int>();

        private static Action<string> _showError;

        private static int _single;

        [Conditional("DEBUG")]
        public static void Start(Action<string> showError)
        {
            var old = Interlocked.Exchange(ref _single, 1);
            if (old != 0)
                throw new NestingException();

            Disposables.Clear();
            _showError = showError;
        }

        [Conditional("DEBUG")]
        public static void End()
        {
            if (Disposables.Any())
            {
                _showError?.Invoke("Found undispose object.");
            }

            Disposables.Clear();

            var old = Interlocked.Exchange(ref _single, 0);
            if (old != 1)
                throw new NestingException();
        }

        [Conditional("DEBUG")]
        public static void Clean()
        {
            _showError = null;
            _single = 0;
            Disposables.Clear();
        }

        [Conditional("DEBUG")]
        public static void Add(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable))
            {
                _showError?.Invoke("Found multiple addition.    -- " + disposable.GetType());
            }

            Disposables[disposable] = 0;
        }

        [Conditional("DEBUG")]
        public static void Remove(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable) == false)
            {
                _showError?.Invoke("Found multiple removing.    -- " + disposable.GetType());
            }

            int dummy;
            Disposables.TryRemove(disposable, out dummy);
        }
    }
}