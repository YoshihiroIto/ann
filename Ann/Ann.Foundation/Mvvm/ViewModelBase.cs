using System.Threading;
using Livet.Messaging;

namespace Ann.Foundation.Mvvm
{
    public class ViewModelBase : ModelBase
    {
        private InteractionMessenger _Messenger;

        public InteractionMessenger Messenger
        {
            get { return LazyInitializer.EnsureInitialized(ref _Messenger, () => new InteractionMessenger()); }
        }
    }
}