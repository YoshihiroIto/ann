using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using SimpleInjector;

namespace Ann.Test
{
    public class TestContext : IDisposable
    {
        private readonly Container _DiContainer = new Container();
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public T GetInstance<T>()where T : class
        {
            return _DiContainer.GetInstance<T>();
        }

        public TestContext()
        {
            _DiContainer.RegisterSingleton(() => new ConfigHolder(_config.RootPath));
            _DiContainer.RegisterSingleton(() => _DiContainer.GetInstance<ConfigHolder>().Config);
            _DiContainer.RegisterSingleton<App>();
            _DiContainer.RegisterSingleton<LanguagesService>();
            _DiContainer.RegisterSingleton<ViewManager>();
            _DiContainer.Register(() => _DiContainer.GetInstance<App>().VersionUpdater);

            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
            _DiContainer.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}