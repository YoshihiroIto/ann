using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MainWindowViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public MainWindowViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Fact]
        public void Basic()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                Assert.True(double.IsNaN(vm.Left.Value));
                Assert.True(double.IsNaN(vm.Top.Value));
                Assert.False(double.IsNaN(vm.CandidatesListMaxHeight.Value));
                Assert.NotNull(vm.Messenger);
                Assert.Equal($"{AssemblyConstants.Product} {AssemblyConstants.Version}", vm.Caption);
                Assert.NotNull(vm.StatusBar);
            }
        }

        [Fact]
        public void SettingShowCommand()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.SettingShowCommand.Execute(null);
            }
        }

        [Fact]
        public void ShowCommand()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.ShowCommand.Execute(null);
            }
        }

        [Fact]
        public void HideCommand()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.HideCommand.Execute(null);
            }
        }

        [Fact]
        public void ExitCommand()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.ExitCommand.Execute(null);
            }
        }

        [Fact]
        public void Candidates()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                Assert.Equal(0, vm.Candidates.Value.Length);
            }
        }
    }
}