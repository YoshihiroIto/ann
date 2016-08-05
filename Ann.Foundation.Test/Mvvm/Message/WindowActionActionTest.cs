using System;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation.Mvvm.Message;
using Xunit;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class WindowActionActionTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

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
        }

        [WpfFact]
        public void MaximizeAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Maximize);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.WindowState, WindowState.Maximized);

            w.Close();
        }

        [WpfFact]
        public void MinimizeAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Minimize);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.WindowState, WindowState.Minimized);

            w.Close();
        }

        [WpfFact]
        public void NormalAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Normal);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.WindowState, WindowState.Normal);

            w.Close();
        }

        [WpfFact]
        public void ActivateAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Active);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.True(w.IsActive);

            w.Close();
        }

        [WpfFact]
        public void VisibleAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Visible);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.Visibility, Visibility.Visible);

            w.Close();
        }

        [WpfFact]
        public void HiddenAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Hidden);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.Visibility, Visibility.Hidden);

            w.Close();
        }

        [WpfFact]
        public void CollapsedAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.Collapsed);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.Visibility, Visibility.Collapsed);

            w.Close();
        }

        [WpfFact]
        public void VisibleActiveAction()
        {
            var w = new Window();
            w.Show();

            var m = new WindowActionMessage(WindowAction.VisibleActive);
            WindowActionAction.InvokeAction(w, m);

            Assert.True(m.IsOk);
            Assert.Equal(w.Visibility, Visibility.Visible);
            Assert.True(w.IsActive);

            w.Close();
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
        }
    }
}