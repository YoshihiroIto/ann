using System;
using System.Collections.Concurrent;
using System.Windows.Threading;
using Ann.Core;
using Ann.Core.Candidate;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class ExecutableFileViewModelTest : IDisposable
    { 
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public ExecutableFileViewModelTest()
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
            var path = AssemblyConstants.EntryAssemblyLocation;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];
            var iconDecoder = new IconDecoder();
            var configHolder = new ConfigHolder(_config.RootPath);

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

                using (var parent = new MainWindowViewModel(app, configHolder))
                using (var vm = new CandidatePanelViewModel(model, app))
                {
                    Assert.Equal("Ann", vm.Name);
                    Assert.Equal(path, vm.Comment);
                    Assert.NotNull(vm.Icon);
                }
            }
        }

        [WpfFact]
        public void PriorityFile()
        {
            var path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Explorer.exe");
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];
            var iconDecoder = new IconDecoder();
            var configHolder = new ConfigHolder(_config.RootPath);

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

                using (var parent = new MainWindowViewModel(app, configHolder))
                using (var vm = new CandidatePanelViewModel(model, app))
                {
                    Assert.False(vm.IsPriorityFile);

                    vm.IsPriorityFile = true;
                    Assert.True(vm.IsPriorityFile);
                    Assert.True(app.IsPriorityFile(path));

                    vm.IsPriorityFile = false;
                    Assert.False(vm.IsPriorityFile);
                    Assert.False(app.IsPriorityFile(path));
                }
            }
        }

        [WpfFact]
        public void IsPriorityFileFlipCommand()
        {
            var path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Explorer.exe");
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];
            var iconDecoder = new IconDecoder();
            var configHolder = new ConfigHolder(_config.RootPath);

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

                using (var parent = new MainWindowViewModel(app, configHolder))
                using (var vm = new CandidatePanelViewModel(model, app))
                {
                    Assert.False(vm.IsPriorityFile);
                    Assert.False(app.IsPriorityFile(path));

                    vm.IsPriorityFileFlipCommand.Execute(null);

                    Assert.True(vm.IsPriorityFile);
                    Assert.True(app.IsPriorityFile(path));

                    vm.IsPriorityFileFlipCommand.Execute(null);

                    Assert.False(vm.IsPriorityFile);
                    Assert.False(app.IsPriorityFile(path));
                }
            }
        }
    }
}