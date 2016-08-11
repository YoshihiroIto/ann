using Ann.Core;
using Ann.SettingWindow.SettingPage.About;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class AboutViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            using (var vm = new AboutViewModel())
            {
               Assert.Equal(VersionCheckingStates.Wait, vm.VersionCheckingState.Value);
               Assert.Equal(0, vm.UpdateProgress.Value);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void RestartCommand()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            using (var vm = new AboutViewModel())
            {
                vm.RestartCommand.Execute(null);
            }

            VersionUpdater.Destory();
        }
    }
}