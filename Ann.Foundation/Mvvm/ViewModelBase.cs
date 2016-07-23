using System.Threading;
using Livet.Messaging;

namespace Ann.Foundation.Mvvm
{
    public class ViewModelBase : DisposableNotificationObject
    {
        private InteractionMessenger _Messenger;

        public InteractionMessenger Messenger
        {
            get { return LazyInitializer.EnsureInitialized(ref _Messenger, () => new InteractionMessenger()); }
        }

        public ViewModelBase(bool disableDisposableChecker = false)
            : base(disableDisposableChecker)
        {
        }
    }
}