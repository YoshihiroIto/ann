using System;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class ConfigHolderTest: IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void SaveConfig()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveConfig();
        }

        [Fact]
        public void SaveMru()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveMru();
        }

        [Fact]
        public void SaveMainWindow()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveMainWindow();
        }
    }
}