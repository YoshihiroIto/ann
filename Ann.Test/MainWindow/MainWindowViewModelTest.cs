using System;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MainWindowViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
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
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.SettingShowCommand.Execute(null);
            }
        }

        [Fact]
        public void ShowCommand()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.ShowCommand.Execute(null);
            }
        }

        [Fact]
        public void HideCommand()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.HideCommand.Execute(null);
            }
        }

        [Fact]
        public void ExitCommand()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                vm.ExitCommand.Execute(null);
            }
        }

        [Fact]
        public void Candidates()
        {
            TestHelper.CleanTestEnv();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new MainWindowViewModel(app, configHolder))
            {
                Assert.Equal(0, vm.Candidates.Value.Length);
            }
        }
    }
}