using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Ann.Foundation
{
    public static class DisposableChecker
    {
        private static readonly ConcurrentDictionary<IDisposable, int> Disposables = new ConcurrentDictionary<IDisposable, int>();

        [Conditional("DEBUG")]
        public static void Start()
        {
        }

        [Conditional("DEBUG")]
        public static void End()
        {
            if (Disposables.Any())
            {
                MessageBox.Show("Found undispose object.");
            }
        }

        [Conditional("DEBUG")]
        public static void Add(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable))
            {
                MessageBox.Show("Found multiple addition.");
            }

            Disposables[disposable] = 0;
        }

        [Conditional("DEBUG")]
        public static void Remove(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable) == false)
            {
                MessageBox.Show("Found multiple diposing.");
            }

            int dummy;
            Disposables.TryRemove(disposable, out dummy);
        }
    }
}