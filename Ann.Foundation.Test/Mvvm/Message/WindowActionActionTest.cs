using System;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation.Mvvm.Message;
using Xunit;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class WindowActionActionTest
    {
        [WpfFact]
        public void NotFoundTopWindow()
        {
            var m = new WindowActionMessage(WindowAction.Close);
            WindowActionAction.InvokeAction(m);

            Assert.False(m.IsOk);
        }

        [WpfFact]
        public void CloseAction()
        {
            var c = 0;

            var w = new Window();
            w.Show();
            w.Closed += (_, __) => c++;

            var m = new WindowActionMessage(WindowAction.Close);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(c, 1);

            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void UnkownAction()
        {
            var c = 0;

            var w = new Window();
            w.Show();
            w.Closed += (_, __) => c++;

            var m = new WindowActionMessage((WindowAction) (-1));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => WindowActionAction.InvokeAction(w, m));

            Assert.False(m.IsOk);
            Assert.Equal(c, 0);

            w.Close();

            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}