using System;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow
{
    public class SettingViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public SettingViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                Assert.Equal(5, vm.Pages.Length);
                Assert.IsType<GeneralViewModel>(vm.SelectedPage.Value);
            }
        }

        [Fact]
        public void InitializeCommand()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.InitializeCommand.Execute(null);
            }
        }

        [Fact]
        public void CloseCommand()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.CloseCommand.Execute(null);
            }
        }
    }
}