using Ann.Core;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage;
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
            using (var vm = new PathViewModel(new Path("AA"), () => string.Empty))
            {
                Assert.Equal("AA", vm.Path.Value);
            }
        }

        [Fact]
        public void FolderSelectDialogOpenCommand()
        {
            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => _.Response = "123"))
            {
                using (var vm = new PathViewModel(new Path("AA"), () => null))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("AA", vm.Path.Value);
                }
            }

            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => _.Response = "123"))
            {
                using (var vm = new PathViewModel(new Path("AA"), () => "456"))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("456", vm.Path.Value);
                }
            }

            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => {}))
            {
                using (var vm = new PathViewModel(new Path("AA"), () => "XYZ"))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("XYZ", vm.Path.Value);
                }
            }
        }
    }
}