using System;
using System.Collections.Concurrent;
using Ann.Core;
using Ann.Core.Candidate;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class ExecutableFileViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [WpfFact]
        public void Basic()
        {
            using (var vm = _context.GetInstance<CandidatePanelViewModel>())
            {
                var path = AssemblyConstants.EntryAssemblyLocation;
                var stringPool = new ConcurrentDictionary<string, string>();
                var targetFolders = new string[0];
                var iconDecoder = new IconDecoder();
                var app = _context.GetInstance<App>();

                vm.Model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

                Assert.Equal("Ann", vm.Name);
                Assert.Equal(path, vm.Comment);
                Assert.NotNull(vm.Icon);
            }
        }

        [WpfFact]
        public void PriorityFile()
        {
            using (var vm = _context.GetInstance<CandidatePanelViewModel>())
            {
                var path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Explorer.exe");
                var stringPool = new ConcurrentDictionary<string, string>();
                var targetFolders = new string[0];
                var iconDecoder = new IconDecoder();
                var app = _context.GetInstance<App>();

                vm.Model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

                Assert.False(vm.IsPriorityFile);

                vm.IsPriorityFile = true;
                Assert.True(vm.IsPriorityFile);
                Assert.True(app.IsPriorityFile(path));

                vm.IsPriorityFile = false;
                Assert.False(vm.IsPriorityFile);
                Assert.False(app.IsPriorityFile(path));
            }
        }

        [WpfFact]
        public void IsPriorityFileFlipCommand()
        {
            using (var vm = _context.GetInstance<CandidatePanelViewModel>())
            {
                var path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Explorer.exe");
                var stringPool = new ConcurrentDictionary<string, string>();
                var targetFolders = new string[0];
                var iconDecoder = new IconDecoder();
                var app = _context.GetInstance<App>();

                vm.Model = new ExecutableFile(path, app, iconDecoder, stringPool, targetFolders);

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