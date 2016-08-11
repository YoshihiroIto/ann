using Ann.Core;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage;
using Reactive.Bindings.Notifiers;
using Xunit;
using TestHelper = Ann.Core.TestHelper;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class PathViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            using (var vm = new PathViewModel(new Path("AA"), false))
            {
                Assert.Equal("AA", vm.Path.Value);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void FolderSelectDialogOpenCommand()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            using (MessageBroker.Default
                .Subscribe<FileOrFolderSelectMessage>(_ => _.Response = "123"))
            {
                using (var vm = new PathViewModel(new Path("AA"), false))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("123", vm.Path.Value);
                }
            }

            using (MessageBroker.Default
                .Subscribe<FileOrFolderSelectMessage>(_ => {}))
            {
                using (var vm = new PathViewModel(new Path("AA"), false))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("AA", vm.Path.Value);
                }
            }

            VersionUpdater.Destory();
        }
    }
}