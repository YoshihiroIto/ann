using System.Threading.Tasks;
using System.Windows.Threading;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann
{
    public class ViewManager : DisposableModelBase
    {
        private readonly Dispatcher _uiDispatcher;

        public ViewManager(Dispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;

            SubscribeMessages();
        }

        private void SubscribeMessages()
        {
            MessageBroker.Default
                .Subscribe<WindowActionMessage>(WindowActionAction.InvokeAction)
                .AddTo(CompositeDisposable);

            MessageBroker.Default
                .Subscribe<FileOrFolderSelectMessage>(FileOrFolderSelectAction.InvokeAction)
                .AddTo(CompositeDisposable);

            AsyncMessageBroker.Default
                .Subscribe<SettingViewModel>(
                    vm => Task.Run(() => _uiDispatcher?.Invoke(() => new SettingWindow.SettingWindow {DataContext = vm}.ShowDialog()))
                ).AddTo(CompositeDisposable);
        }
    }
}