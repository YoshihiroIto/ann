using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace Ann.Foundation
{
    public static class DisposableChecker
    {
        private static readonly ConcurrentDictionary<IDisposable, int> Disposables = new ConcurrentDictionary<IDisposable, int>();

        private static Action<string> _showError;

        [Conditional("DEBUG")]
        public static void Start(Action<string> showError)
        {
            _showError = showError;
        }

        [Conditional("DEBUG")]
        public static void End()
        {
            if (Disposables.Any())
            {
                _showError?.Invoke("Found undispose object.");
            }
        }

        [Conditional("DEBUG")]
        public static void Add(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable))
            {
                _showError?.Invoke("Found multiple addition." + disposable.GetType());
            }

            Disposables[disposable] = 0;
        }

        [Conditional("DEBUG")]
        public static void Remove(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable) == false)
            {
                _showError?.Invoke("Found multiple diposing." + disposable.GetType());
            }

            int dummy;
            Disposables.TryRemove(disposable, out dummy);
        }
    }
}