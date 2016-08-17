using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Xunit;

namespace Ann.Foundation.Test
{
    public class HotKeyRegisterTest : IDisposable
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

            using (var r = new HotKeyRegister(ModifierKeys.Alt, Key.Up, w))
            {
                Assert.True(r.Register());

                r.Unregister();
            }

            using (var r = new HotKeyRegister(ModifierKeys.Alt, Key.Up, w))
            {
                Assert.True(r.Register());
            }
        }

        [WpfFact]
        public void AlreadyRegister()
        {
            var w = new Window();

            using (var r = new HotKeyRegister(ModifierKeys.Alt, Key.Up, w))
            {
                Assert.True(r.Register());
                Assert.True(r.Register());
            }
        }

        [WpfFact]
        public void AlreadyUsed()
        {
            var w = new Window();

            using (var r1 = new HotKeyRegister(ModifierKeys.Alt, Key.Up, w))
            {
                Assert.True(r1.Register());

                using (var r2 = new HotKeyRegister(ModifierKeys.Alt, Key.Up, w))
                {
                    Assert.False(r2.Register());
                }
            }
        }
    }
}