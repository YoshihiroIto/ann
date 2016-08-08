using System;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.About;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class AboutTestFixture : IDisposable
    {
        public AboutTestFixture()
        {
            TestHelper.SetEntryAssembly();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class AboutTest : IClassFixture<AboutTestFixture>,  IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public AboutTest(AboutTestFixture f)
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

            using (var vm = new AboutViewModel())
            {
               Assert.Equal(VersionCheckingStates.Wait, vm.VersionCheckingState.Value);
               Assert.Equal(0, vm.UpdateProgress.Value);


                vm.CheckVersionAsync().Wait();
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void RestartCommand()
        {
            VersionUpdater.Initialize();

            using (var vm = new AboutViewModel())
            {
                vm.RestartCommand.Execute(null);
            }

            VersionUpdater.Destory();
        }
    }
}