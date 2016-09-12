using System;
using System.Windows.Threading;
using Ann.Foundation;
using SimpleInjector;

namespace Ann.Core
{
    public class TestContext : IDisposable
    {
        private readonly Container _DiContainer = new Container();
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public string ConfigRootPath => _config.RootPath;

        public T GetInstance<T>() where T : class
        {
            return _DiContainer.GetInstance<T>();
        }

        public TestContext()
        {
            _DiContainer.RegisterSingleton(() => new ConfigHolder(_config.RootPath));
            _DiContainer.RegisterSingleton(() => _DiContainer.GetInstance<ConfigHolder>().Config);
            _DiContainer.RegisterSingleton<App>();
            _DiContainer.RegisterSingleton<LanguagesService>();
            _DiContainer.Register(() => _DiContainer.GetInstance<App>().VersionUpdater);

            TestHelper.CleanTestEnv();
        }

        private void Release()
        {
            _config.Dispose();
            _DiContainer.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        #region IDisposable

        private bool _isDisposed;

        ~TestContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                Release();

            _isDisposed = true;
        }

        #endregion
    }
}