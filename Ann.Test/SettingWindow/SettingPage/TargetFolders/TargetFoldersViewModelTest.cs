using System;
using System.Threading;
using Ann.Core;
using Ann.Foundation;
using Ann.Properties;
using Ann.SettingWindow.SettingPage;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Xunit;
using App = Ann.Core.App;

namespace Ann.Test.SettingWindow.SettingPage.TargetFolders
{
    public class TargetFoldersViewModelTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public TargetFoldersViewModelTest()
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

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                Assert.True(vm.IsIncludeSystemFolder.Value);
                Assert.True(vm.IsIncludeSystemX86Folder.Value);
                Assert.True(vm.IsIncludeProgramsFolder.Value);
                Assert.True(vm.IsIncludeProgramFilesFolder.Value);
                Assert.True(vm.IsIncludeProgramFilesX86Folder.Value);
                Assert.True(vm.IsIncludeCommonStartMenuFolder.Value);

                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                Assert.False(vm.IsIncludeSystemFolder.Value);
                Assert.False(vm.IsIncludeSystemX86Folder.Value);
                Assert.False(vm.IsIncludeProgramsFolder.Value);
                Assert.False(vm.IsIncludeProgramFilesFolder.Value);
                Assert.False(vm.IsIncludeProgramFilesX86Folder.Value);
                Assert.False(vm.IsIncludeCommonStartMenuFolder.Value);

                Assert.False(model.TargetFolder.IsIncludeSystemFolder);
                Assert.False(model.TargetFolder.IsIncludeSystemX86Folder);
                Assert.False(model.TargetFolder.IsIncludeProgramsFolder);
                Assert.False(model.TargetFolder.IsIncludeProgramFilesFolder);
                Assert.False(model.TargetFolder.IsIncludeProgramFilesX86Folder);
                Assert.False(model.TargetFolder.IsIncludeCommonStartMenu);

                Assert.Equal(0, vm.Folders.Count);
                model.TargetFolder.Folders.Add(new Path("AA"));
                Assert.Equal(1, vm.Folders.Count);
                Assert.Equal("AA", vm.Folders[0].Path.Value);
            }
        }

        [Fact]
        public void FolderAddCommand()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                vm.FolderAddCommand.Execute();

                Assert.Equal(1, vm.Folders.Count);
                Assert.Equal(1, model.TargetFolder.Folders.Count);

                Assert.Equal(string.Empty, vm.Folders[0].Path.Value);
                Assert.Equal(string.Empty, model.TargetFolder.Folders[0].Value);
            }
        }

        [Fact]
        public void FolderRemoveCommand()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                model.TargetFolder.Folders.Add(new Path("AA"));
                model.TargetFolder.Folders.Add(new Path("BB"));
                model.TargetFolder.Folders.Add(new Path("CC"));

                Assert.Equal(3, vm.Folders.Count);
                Assert.Equal(3, model.TargetFolder.Folders.Count);

                vm.FolderRemoveCommand.Execute(vm.Folders[1]);

                Assert.Equal(2, vm.Folders.Count);
                Assert.Equal(2, model.TargetFolder.Folders.Count);

                Assert.Equal("AA", vm.Folders[0].Path.Value);
                Assert.Equal("CC", vm.Folders[1].Path.Value);
                Assert.Equal("AA", model.TargetFolder.Folders[0].Value);
                Assert.Equal("CC", model.TargetFolder.Folders[1].Value);
            }
        }

        [Fact]
        public void FoldersValidate_FolderNotFound()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                model.TargetFolder.Folders.Add(new Path("XYZ"));

                while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value))
                    Thread.Sleep(TimeSpan.FromMilliseconds(20));

                Assert.Equal(Resources.Message_FolderNotFound, vm.Folders[0].ValidationMessage.Value);
            }
        }

        [Fact]
        public void FoldersValidate_FolderFound()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                model.TargetFolder.Folders.Add(new Path(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));

                while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value) == false)
                    Thread.Sleep(TimeSpan.FromMilliseconds(20));

                Assert.Null(vm.Folders[0].ValidationMessage.Value);
            }
        }

        [Fact]
        public void FoldersValidate_FolderNotFoundToFound()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                {
                    model.TargetFolder.Folders.Add(new Path("XYZ"));

                    while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value))
                        Thread.Sleep(TimeSpan.FromMilliseconds(20));

                    Assert.Equal(Resources.Message_FolderNotFound, vm.Folders[0].ValidationMessage.Value);
                }

                {
                    model.TargetFolder.Folders[0].Value = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value) == false)
                        Thread.Sleep(TimeSpan.FromMilliseconds(20));

                    Assert.Null(vm.Folders[0].ValidationMessage.Value);
                }
            }
        }

        [Fact]
        public void FoldersValidate_FolderFoundToNotFound()
        {
            var model = new Core.Config.App();

            using (var app = new App(new ConfigHolder(_config.RootPath)))
            using (var vm = new TargetFoldersViewModel(model, app))
            {
                vm.IsIncludeSystemFolder.Value = false;
                vm.IsIncludeSystemX86Folder.Value = false;
                vm.IsIncludeProgramsFolder.Value = false;
                vm.IsIncludeProgramFilesFolder.Value = false;
                vm.IsIncludeProgramFilesX86Folder.Value = false;
                vm.IsIncludeCommonStartMenuFolder.Value = false;

                {
                    model.TargetFolder.Folders.Add(new Path(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));

                    while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value) == false)
                        Thread.Sleep(TimeSpan.FromMilliseconds(20));

                    Assert.Null(vm.Folders[0].ValidationMessage.Value);
                }

                {
                    model.TargetFolder.Folders[0].Value = "XYZ";

                    while (string.IsNullOrEmpty(vm.Folders[0].ValidationMessage.Value))
                        Thread.Sleep(TimeSpan.FromMilliseconds(20));

                    Assert.Equal(Resources.Message_FolderNotFound, vm.Folders[0].ValidationMessage.Value);
                }
            }
        }
    }
}