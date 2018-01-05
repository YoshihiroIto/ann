using Xunit;

namespace Ann.Core.Test
{
    public class VersionUpdaterTest
    {
        public VersionUpdaterTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void BasicOnDisabledSilentUpdate()
        {
            using (var versionUpdater = new VersionUpdater(null))
            {
                Assert.False(versionUpdater.IsEnableSilentUpdate);
                Assert.False(versionUpdater.IsAvailableUpdate);
                Assert.Equal(0, versionUpdater.UpdateProgress);

                Assert.Equal(VersionCheckingStates.Wait, versionUpdater.VersionCheckingState);

                Assert.False(versionUpdater.IsRestartRequested);
                versionUpdater.RequestRestart();
                Assert.False(versionUpdater.IsRestartRequested);

                versionUpdater.CheckAsync().Wait();

                versionUpdater.CreateStartupShortcut().Wait();
                versionUpdater.RemoveStartupShortcut().Wait();
            }
        }

        [Fact]
        public void Restart()
        {
            using (var versionUpdater = new VersionUpdater(null))
            {
                if (versionUpdater.IsRestartRequested)
                    VersionUpdater.Restart();
            }
        }
    }
}