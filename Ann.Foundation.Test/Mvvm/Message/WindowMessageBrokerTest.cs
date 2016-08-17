using System;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation.Mvvm.Message;
using Xunit;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class WindowMessageBrokerTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        public class TestMessage
        {
            public int Param0 { get; set; }

            public Window Window { get; set; }
        }

        [WpfFact]
        public void Basic()
        {
            var c = 0;

            using (var mb = new WindowMessageBroker())
            {
                var w = new Window();
                mb.Window = w;
                mb.Subscribe<TestMessage>((window, m) =>
                {
                    c = m.Param0;
                    m.Window = window;
                });

                var mes = new TestMessage {Param0 = 123};

                mb.Publish(mes);

                Assert.Equal(123, c);
                Assert.Same(mes.Window, w);
            }
        }
    }
}
