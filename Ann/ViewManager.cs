using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ann.Foundation.Exception;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann
{
    public class ViewManager : DisposableModelBase
    {
        private static ViewManager _Instance;
        public static ViewManager Instance
        {
            get
            {
                if (_Instance == null)
                    throw new UninitializedException();

                return _Instance;
            }
        }

        public Dispatcher UiDispatcher { get; private set; }

        public static void Clean()
        {
            _Instance?.Dispose();
            _Instance = null;
        }

        public static void Initialize(Dispatcher uiDispatcher)
        {
            Debug.Assert(uiDispatcher != null);

            if (_Instance != null)
                throw new NestingException();

            _Instance = new ViewManager();

            Instance.UiDispatcher = uiDispatcher;

            Instance.SubscribeMessages();
        }

        public static void Destory()
        {
            if (_Instance == null)
                throw new NestingException();

            Clean();
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