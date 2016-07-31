using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Ann.Foundation.Mvvm.Message
{
    public static class WindowActionAction
    {
        public static void InvokeAction(WindowActionMessage message)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive);

            InvokeAction(window, message);
        }

        public static void InvokeAction(Window window, WindowActionMessage message)
        {
            message.IsOk = true;

            if (window == null)
            {
                message.IsOk = false;
                return;
            }

            switch (message.Action)
            {
                case WindowAction.Close:
                    window.Close();
                    break;

                case WindowAction.Maximize:
                    window.WindowState = WindowState.Maximized;
                    break;

                case WindowAction.Minimize:
                    window.WindowState = WindowState.Minimized;
                    break;

                case WindowAction.Normal:
                    window.WindowState = WindowState.Normal;
                    break;

                case WindowAction.Active:
                    window.Activate();
                    window.Focus();
                    break;

                case WindowAction.Visible:
                    window.Visibility = Visibility.Visible;
                    break;

                case WindowAction.Hidden:
                    window.Visibility = Visibility.Hidden;
                    break;

                case WindowAction.Collapsed:
                    window.Visibility = Visibility.Collapsed;
                    break;

                case WindowAction.VisibleActive:
                    window.Visibility = Visibility.Visible;
                    window.Activate();
                    window.Focus();
                    break;

                default:
                    message.IsOk = false;
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}