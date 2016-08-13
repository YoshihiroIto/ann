using Ann.Core;
using Ann.SettingWindow;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow
{
    public class SettingViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var model = new Core.Config.App();

            using (var app = new App())
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
            TestHelper.CleanTestEnv();

            var model = new Core.Config.App();

            using (var app = new App())
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.InitializeCommand.Execute(null);
            }
        }

        [Fact]
        public void CloseCommand()
        {
            TestHelper.CleanTestEnv();

            var model = new Core.Config.App();

            using (var app = new App())
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.CloseCommand.Execute(null);
            }
        }
    }
}