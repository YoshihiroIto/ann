using System;
using Ann.Foundation;
using Ann.Foundation.Exception;
using Xunit;

namespace Ann.Core.Test
{
    public class VersionUpdaterTestFixture : IDisposable
    {
        public VersionUpdaterTestFixture()
        {
            TestHelper.SetEntryAssembly();

            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class VersionUpdaterTest : IClassFixture<VersionUpdaterTestFixture>, IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public VersionUpdaterTest(VersionUpdaterTestFixture f)
        {
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }

        [Fact]
        public void BasicOnDisabledSilentUpdate()
        {
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
            VersionUpdater.Initialize();

            Assert.Throws<NestingException>(() =>
                VersionUpdater.Initialize());
        }

        [Fact]
        public void NestingDestory()
        {
            Assert.Throws<NestingException>(() =>
                VersionUpdater.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            Assert.Throws<UninitializedException>(() =>
                VersionUpdater.Instance.IsAvailableUpdate);
        }
    }
}