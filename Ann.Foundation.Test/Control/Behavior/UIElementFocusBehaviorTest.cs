using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ann.Foundation.Control.Behavior;
using Xunit;

namespace Ann.Foundation.Test.Control.Behavior
{
    // ReSharper disable once InconsistentNaming
    public class UIElementFocusBehaviorTest : IDisposable
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

            var button = new Button();
            w.Content = button;

            var b = new UIElementFocusBehavior();

            b.Attach(button);

            Assert.False(b.IsFocused);

            w.Show();
            button.Focusable = true;
            button.Focus();

            Assert.True(b.IsFocused);

            w.Close();

            b.Detach();
        }
    }
}