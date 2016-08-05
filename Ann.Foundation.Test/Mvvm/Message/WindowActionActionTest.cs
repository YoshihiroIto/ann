using System;
using System.Windows;
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
            try
            {
                var c = 0;

                var w = new Window();
                w.Show();
                w.Closed += (_, __) => c++;

                var m = new WindowActionMessage(WindowAction.Close);
                WindowActionAction.InvokeAction(w, m);

                Assert.True(m.IsOk);
                Assert.Equal(c, 1);
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            {
                // ignored for appveyor
            }
        }

        [WpfFact]
        public void UnkownAction()
        {
            try
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
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            {
                // ignored for appveyor
            }
        }
    }
}