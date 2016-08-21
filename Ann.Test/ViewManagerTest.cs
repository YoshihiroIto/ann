using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Xunit;

namespace Ann.Test
{
    public class ViewManagerTest :  IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public ViewManagerTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languageService = new LanguagesService(configHolder.Config))
            using (new ViewManager(Dispatcher.CurrentDispatcher, languageService))
            {
            }
        }
    }
}