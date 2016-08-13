using System;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _config.Dispose();
        }

        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (new StatusBarViewModel(app))
            {
            }
        }

        [WpfFact]
        public void Messages()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
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
                    vm.Messages.Add(new ProcessingStatusBarItemViewModel("aaa"));
                    Assert.Equal(1, vm.Messages.Count);
                }
            }
        }

        [WpfFact]
        public void Visibility()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
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

                    var i = new ProcessingStatusBarItemViewModel("aaa");
                    vm.Messages.Add(i);
                    Assert.Equal(System.Windows.Visibility.Visible, vm.Visibility.Value);

                    vm.Messages.Remove(i);
                    Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);
                }
            }
        }
    }
}