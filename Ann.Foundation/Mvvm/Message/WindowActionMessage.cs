using System.Windows;

namespace Ann.Foundation.Mvvm.Message
{
    public enum WindowAction
    {
        Close,
        Maximize,
        Minimize,
        Normal,
        Active,
        //
        Visible,
        Hidden,
        Collapsed,
        //
        VisibleActive
    }

    public class WindowActionMessage
    {
        public WindowAction Action { get; }

        public bool IsOk { get; set; }

        public WindowActionMessage(WindowAction a)
        {
            Action = a;
        }
    }
}