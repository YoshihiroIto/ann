using Ann.Foundation.Exception;
using Xunit;

namespace Ann.Core.Test
{
    public class VersionUpdaterTest
    {
        [Fact]
        public void BasicOnDisabledSilentUpdate()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            Assert.False(VersionUpdater.Instance.IsEnableSilentUpdate);
            Assert.False(VersionUpdater.Instance.IsAvailableUpdate);
            Assert.Equal(0, VersionUpdater.Instance.UpdateProgress);

            Assert.Equal(VersionCheckingStates.Wait, VersionUpdater.Instance.VersionCheckingState);

            Assert.False(VersionUpdater.Instance.IsRestartRequested);
            VersionUpdater.Instance.RequestRestart();
            Assert.False(VersionUpdater.Instance.IsRestartRequested);

            VersionUpdater.Instance.CheckAsync().Wait();

            VersionUpdater.Instance.CreateStartupShortcut().Wait();
            VersionUpdater.Instance.RemoveStartupShortcut().Wait();

            VersionUpdater.Destory();
        }

        [Fact]
        public void NestingInitialize()
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();

            Assert.Throws<NestingException>(() =>
                VersionUpdater.Initialize());
        }

        [Fact]
        public void NestingDestory()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<NestingException>(() =>
                VersionUpdater.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<UninitializedException>(() =>
                VersionUpdater.Instance.IsAvailableUpdate);
        }
    }
}