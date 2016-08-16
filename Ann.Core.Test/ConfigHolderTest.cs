using System;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class ConfigHolderTest: IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public ConfigHolderTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void SaveConfig()
        {
            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveConfig();
        }

        [Fact]
        public void SaveMru()
        {
            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveMru();
        }

        [Fact]
        public void SaveMainWindow()
        {
            var configHolder = new ConfigHolder(_config.RootPath);

            configHolder.SaveMainWindow();
        }
    }
}