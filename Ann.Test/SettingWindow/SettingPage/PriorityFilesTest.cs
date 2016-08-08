using System;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class PriorityFilesTestFixture : IDisposable
    {
        public PriorityFilesTestFixture()
        {
            TestHelper.SetEntryAssembly();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class PriorityFilesTest : IClassFixture<PriorityFilesTestFixture>,  IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public PriorityFilesTest(PriorityFilesTestFixture f)
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

            var app = new Core.Config.App();
            using (new PriorityFilesViewModel(app))
            {
            }

            VersionUpdater.Destory();
        }
    }
}