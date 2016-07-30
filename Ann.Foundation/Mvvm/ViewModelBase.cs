namespace Ann.Foundation.Mvvm
{
    public class ViewModelBase : DisposableNotificationObject
    {
        public ViewModelBase(bool disableDisposableChecker = false)
            : base(disableDisposableChecker)
        {
        }
    }
}