using Ann.Foundation.Mvvm.Message;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow
    {
        public SettingWindow()
        {
            InitializeComponent();

            Loaded += (_, __) => SetupMessenger();
        }

        private void SetupMessenger()
        {
            var vm = (SettingViewModel) DataContext;

            vm.Messenger.Window = this;

            vm.Messenger
                .Subscribe<WindowActionMessage>(WindowActionAction.InvokeAction)
                .AddTo(vm.CompositeDisposable);

            vm.Messenger
                .Subscribe<FileOrFolderSelectMessage>(FileOrFolderSelectAction.InvokeAction)
                .AddTo(vm.CompositeDisposable);
        }
    }
}