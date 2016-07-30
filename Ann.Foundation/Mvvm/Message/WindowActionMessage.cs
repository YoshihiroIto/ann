namespace Ann.Foundation.Mvvm.Message
{
        public enum WindowAction
        {
            Close,
            Maximize,
            Minimize,
            Normal,
            Active,
        }

    public class WindowActionMessage
    {
        public WindowAction Action { get; set; }
    }
}