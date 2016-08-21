using System;
using Ann.Core;
using Ann.Foundation;
using Ann.Properties;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.PriorityFiles
{
    public class PriorityFilesViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public PriorityFilesViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (new PriorityFilesViewModel(model, app))
            {
            }
        }

        [Fact]
        public void Files()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                Assert.Equal(0, vm.Files.Count);

                model.PriorityFiles.Add(new Path("AA"));
                Assert.Equal(1, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);

                model.PriorityFiles.Add(new Path("BB"));
                Assert.Equal(2, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);
                Assert.Equal("BB", vm.Files[1].Path.Value);

                model.PriorityFiles.RemoveAt(0);
                Assert.Equal(1, vm.Files.Count);
                Assert.Equal("BB", vm.Files[0].Path.Value);
            }
        }

        [Fact]
        public void FileAddCommand()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                vm.FileAddCommand.Execute(null);

                Assert.Equal(1, vm.Files.Count);
                Assert.Equal(string.Empty, vm.Files[0].Path.Value);

                Assert.Equal(1, model.PriorityFiles.Count);
                Assert.Equal(string.Empty, model.PriorityFiles[0].Value);
            }
        }

        [Fact]
        public void FileRemoveCommand()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                Assert.Equal(0, vm.Files.Count);

                var aa = new Path("AA");
                var bb = new Path("BB");
                var cc = new Path("CC");

                model.PriorityFiles.Add(aa);
                model.PriorityFiles.Add(bb);
                model.PriorityFiles.Add(cc);

                vm.FileRemoveCommand.Execute(vm.Files[1]);

                Assert.Equal(2, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);
                Assert.Equal("CC", vm.Files[1].Path.Value);

                Assert.Equal(2, model.PriorityFiles.Count);
                Assert.Equal("AA", model.PriorityFiles[0].Value);
                Assert.Equal("CC", model.PriorityFiles[1].Value);
            }
        }

        [Fact]
        public void PathFolderSelect()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                model.PriorityFiles.Add(new Path("AA"));
                vm.Files[0].FolderSelectDialogOpenCommand.Execute(null);
            }
        }

        [Fact]
        public void IsFocused_Empty_AutoRemove()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                model.PriorityFiles.Add(new Path(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\explorer.exe")));

                Assert.Equal(1, vm.Files.Count);

                vm.Files[0].IsFocused.Value = true;
                vm.Files[0].Path.Value = string.Empty;

                vm.Files[0].IsFocused.Value = false;

                Assert.Equal(0, model.PriorityFiles.Count);
            }
        }

        [Fact]
        public void Validate_AlreadySetSameFile()
        {
            var model = new Core.Config.App();

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var vm = new PriorityFilesViewModel(model, app))
            {
                model.PriorityFiles.Add(new Path(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\explorer.exe")));
                model.PriorityFiles.Add(new Path(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\notepad.exe")));

                Assert.Null(vm.Files[0].ValidationMessage.Value);
                Assert.Null(vm.Files[1].ValidationMessage.Value);

                model.PriorityFiles.Add(new Path(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\explorer.exe")));

                Assert.Equal(Resources.Message_AlreadySetSameFile, vm.Files[0].ValidationMessage.Value);
                Assert.Null(vm.Files[1].ValidationMessage.Value);
                Assert.Equal(Resources.Message_AlreadySetSameFile, vm.Files[2].ValidationMessage.Value);
            }
        }
    }
}