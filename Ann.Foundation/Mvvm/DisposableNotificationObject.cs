using System;
using System.Reactive.Disposables;
using System.Threading;

namespace Ann.Foundation.Mvvm
{
    public class DisposableNotificationObject : NotificationObject, IDisposable
    {
        private CompositeDisposable _compositeDisposable;

        public CompositeDisposable CompositeDisposable
        {
            get { return LazyInitializer.EnsureInitialized(ref _compositeDisposable, () => new CompositeDisposable()); }
        }

        private readonly bool _disableDisposableChecker;

        public DisposableNotificationObject(bool disableDisposableChecker = false)
        {
            _disableDisposableChecker = disableDisposableChecker;

            if (_disableDisposableChecker == false)
                DisposableChecker.Add(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _compositeDisposable?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            if (_disableDisposableChecker == false)
                DisposableChecker.Remove(this);
        }
    }
}