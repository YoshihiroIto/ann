using System;
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

            if (window == null)
                return;

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

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}