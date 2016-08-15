using Xunit;

namespace Ann.Core.Test
{
    public class VersionUpdaterTest
    {
        [Fact]
        public void BasicOnDisabledSilentUpdate()
        {
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

            using (var versionUpdater = new VersionUpdater(null))
            {
                versionUpdater.Restart();
            }
        }
    }
}