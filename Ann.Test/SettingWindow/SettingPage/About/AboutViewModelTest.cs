using System;
using Ann.Core;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage.About;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.About
{
    public class AboutViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var messenger = new WindowMessageBroker())
            using (var vm = new AboutViewModel(versionUpdater, messenger))
            {
               Assert.Equal(VersionCheckingStates.Wait, vm.VersionCheckingState.Value);
               Assert.Equal(0, vm.UpdateProgress.Value);
            }
        }

        [Fact]
        public void RestartCommand()
        {
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var messenger = new WindowMessageBroker())
            using (var vm = new AboutViewModel(versionUpdater, messenger))
            {
                vm.RestartCommand.Execute(null);
            }
        }
    }
}