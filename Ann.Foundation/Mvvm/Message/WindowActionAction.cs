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
            Debug.Assert(window != null);

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

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}