using System;
using System.Windows;
using Reactive.Bindings.Notifiers;

namespace Ann.Foundation.Mvvm.Message
{
    public class WindowMessageBroker : MessageBroker
    {
        public Window Window { get; set; }

        public IDisposable Subscribe<T>(Action<Window, T> action)
        {
            return Subscribe<T>(m => action(Window, m));
        }
    }
}