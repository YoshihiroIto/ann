using System;
using System.Reactive.Disposables;

namespace Ann.Foundation
{
    public static class CompositDisposableExtension
    {
        public static void Add(this CompositeDisposable c, Action action)
        {
            c.Add(Disposable.Create(action));
        }
    }
}