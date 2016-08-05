using System.Windows;
using Ann.Foundation.Mvvm.Message;
using Xunit;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class WindowMessageBrokerTest
    {
        public class TestMessage
        {
            public int Param0 { get; set; }

            public Window Window { get; set; }
        }

        [WpfFact]
        public void Simple()
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

                Assert.Equal(c, 123);
                Assert.Same(mes.Window, w);
            }
        }
    }
}
