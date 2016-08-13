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

            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(app, versionUpdater))
            {
                Assert.Equal(5, vm.Pages.Length);
                Assert.IsType<GeneralViewModel>(vm.SelectedPage.Value);
            }
        }

        [Fact]
        public void InitializeCommand()
        {
            TestHelper.CleanTestEnv();

            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(app, versionUpdater))
            {
                vm.InitializeCommand.Execute(null);
            }
        }

        [Fact]
        public void CloseCommand()
        {
            TestHelper.CleanTestEnv();

            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new SettingViewModel(app, versionUpdater))
            {
                vm.CloseCommand.Execute(null);
            }
        }
    }
}