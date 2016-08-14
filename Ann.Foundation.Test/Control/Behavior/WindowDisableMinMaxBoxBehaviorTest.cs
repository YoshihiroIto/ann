using System;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation.Control.Behavior;
using Xunit;

namespace Ann.Foundation.Test.Control.Behavior
{
    public class WindowDisableMinMaxBoxBehaviorTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            var w = new Window();

            var b = new WindowDisableMinMaxBoxBehavior();
            b.Attach(w);

            w.Show();
            w.Close();

            b.Detach();
        }
    }
}