using Ann.Core;
using Ann.SettingWindow.SettingPage.About;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.About
{
    public class AboutViewModelTest
    {
        public AboutViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void Basic()
        {
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new AboutViewModel(versionUpdater))
            {
               Assert.Equal(VersionCheckingStates.Wait, vm.VersionCheckingState.Value);
               Assert.Equal(0, vm.UpdateProgress.Value);
            }
        }

        [Fact]
        public void RestartCommand()
        {
            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new AboutViewModel(versionUpdater))
            {
                vm.RestartCommand.Execute(null);
            }
        }
    }
}