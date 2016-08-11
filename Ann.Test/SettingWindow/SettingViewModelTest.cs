using Ann.Core;
using Ann.SettingWindow;
using Ann.SettingWindow.SettingPage.General;
using Xunit;
using TestHelper = Ann.Core.TestHelper;

namespace Ann.Test.SettingWindow
{
    public class SettingViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new SettingViewModel(app))
            {
                Assert.Equal(5, vm.Pages.Length);
                Assert.IsType<GeneralViewModel>(vm.SelectedPage.Value);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void InitializeCommand()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new SettingViewModel(app))
            {
                vm.InitializeCommand.Execute(null);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void CloseCommand()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new SettingViewModel(app))
            {
                vm.CloseCommand.Execute(null);
            }

            VersionUpdater.Destory();
        }
    }
}