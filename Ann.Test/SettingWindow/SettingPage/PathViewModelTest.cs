using Ann.Core;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage;
using Reactive.Bindings.Notifiers;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class PathViewModelTest
    {
        public PathViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void Basic()
        {
            using (var vm = new PathViewModel(new Path("AA"), false))
            {
                Assert.Equal("AA", vm.Path.Value);
            }
        }

        [Fact]
        public void FolderSelectDialogOpenCommand()
        {
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
        }
    }
}