using System;
using System.Windows;
using System.Windows.Threading;
using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowHelperTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void EnableBlur()
        {
            // 例外にならない
            var w = new Window();
            WindowHelper.EnableBlur(w);
        }
    }
}
