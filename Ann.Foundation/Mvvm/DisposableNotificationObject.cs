using System;
using System.Threading;
using Livet;

namespace Ann.Foundation.Mvvm
{
    public class DisposableNotificationObject : NotificationObject, IDisposable
    {
        private LivetCompositeDisposable _compositeDisposable;

        public LivetCompositeDisposable CompositeDisposable
        {
            get { return LazyInitializer.EnsureInitialized(ref _compositeDisposable, () => new LivetCompositeDisposable()); }
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