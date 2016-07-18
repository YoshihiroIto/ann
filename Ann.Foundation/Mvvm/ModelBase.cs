using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Livet;
using YamlDotNet.Serialization;

namespace Ann.Foundation.Mvvm
{
    public class ModelBase : NotificationObject, IDisposable
    {
        private LivetCompositeDisposable _compositeDisposable;

        [YamlIgnore]
        public LivetCompositeDisposable CompositeDisposable
        {
            get { return LazyInitializer.EnsureInitialized(ref _compositeDisposable, () => new LivetCompositeDisposable()); }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
 
            storage = value;

            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);

            return true;
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
        }
    }
}