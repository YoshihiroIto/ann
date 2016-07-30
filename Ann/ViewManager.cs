using System.Diagnostics;
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
        public static ViewManager Instance { get; } = new ViewManager();

        public Dispatcher UiDispatcher { get; private set; }

        public static void Initialize(Dispatcher uiDispatcher)
        {
            Debug.Assert(uiDispatcher != null);

            Instance.UiDispatcher = uiDispatcher;

            Instance.SubscribeMessages();
        }

        public static void Destory()
        {
            Instance.Dispose();
        }

        private ViewManager()
        {
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
                    vm => Task.Run(() => UiDispatcher.Invoke(() => new SettingWindow.SettingWindow {DataContext = vm}.ShowDialog()))
                ).AddTo(CompositeDisposable);
        }
    }
}