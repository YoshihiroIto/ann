using System;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage;
using Reactive.Bindings.Notifiers;
using Xunit;

namespace Ann.Test.SettingWindow
{
    public class PathViewModeTestFixture : IDisposable
    {
        public PathViewModeTestFixture()
        {
            TestHelper.SetEntryAssembly();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class PathViewModelTest : IClassFixture<PathViewModeTestFixture>, IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public PathViewModelTest(PathViewModeTestFixture f)
        {
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }

        [Fact]
        public void Basic()
        {
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