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
            TestHelper.CleanTestEnv();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new AboutViewModel(versionUpdater))
            {
                vm.RestartCommand.Execute(null);
            }
        }
    }
}