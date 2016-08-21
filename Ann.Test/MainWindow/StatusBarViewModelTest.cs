using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public StatusBarViewModelTest()
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
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (new StatusBarViewModel(app))
            {
            }
        }

        [WpfFact]
        public void Messages()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                using (var vm = new StatusBarViewModel(app))
                {
                    Assert.Equal(0, vm.Messages.Count);
                    vm.Messages.Add(new ProcessingStatusBarItemViewModel(app, StringTags.AllFiles));
                    Assert.Equal(1, vm.Messages.Count);
                }
            }
        }

        [WpfFact]
        public void Visibility()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                using (var vm = new StatusBarViewModel(app))
                {
                    Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);

                    var i = new ProcessingStatusBarItemViewModel(app, StringTags.AllFiles);
                    vm.Messages.Add(i);
                    Assert.Equal(System.Windows.Visibility.Visible, vm.Visibility.Value);

                    vm.Messages.Remove(i);
                    Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);
                }
            }
        }
    }
}