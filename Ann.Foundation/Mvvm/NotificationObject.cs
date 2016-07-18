using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ann.Foundation.Mvvm
{
    public class NotificationObject : Livet.NotificationObject
    {
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;

            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}