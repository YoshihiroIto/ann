using Ann.Core;
using Ann.SettingWindow.SettingPage;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Xunit;
using App = Ann.Core.App;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class TargetFoldersViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (var vm = new TargetFoldersViewModel(app))
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

                Assert.False(app.TargetFolder.IsIncludeSystemFolder);
                Assert.False(app.TargetFolder.IsIncludeSystemX86Folder);
                Assert.False(app.TargetFolder.IsIncludeProgramsFolder);
                Assert.False(app.TargetFolder.IsIncludeProgramFilesFolder);
                Assert.False(app.TargetFolder.IsIncludeProgramFilesX86Folder);
                Assert.False(app.TargetFolder.IsIncludeCommonStartMenu);

                Assert.Equal(0, vm.Folders.Count);
                app.TargetFolder.Folders.Add(new Path("AA"));
                Assert.Equal(1, vm.Folders.Count);
                Assert.Equal("AA", vm.Folders[0].Path.Value);
            }

            App.Destory();
        }

        [Fact]
        public void FolderAddCommand()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (var vm = new TargetFoldersViewModel(app))
            {
                vm.FolderAddCommand.Execute();

                Assert.Equal(1, vm.Folders.Count);
                Assert.Equal(1, app.TargetFolder.Folders.Count);

                Assert.Equal(string.Empty, vm.Folders[0].Path.Value);
                Assert.Equal(string.Empty, app.TargetFolder.Folders[0].Value);
            }

            App.Destory();
        }

        [Fact]
        public void FolderRemoveCommand()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (var vm = new TargetFoldersViewModel(app))
            {
                app.TargetFolder.Folders.Add(new Path("AA"));
                app.TargetFolder.Folders.Add(new Path("BB"));
                app.TargetFolder.Folders.Add(new Path("CC"));

                Assert.Equal(3, vm.Folders.Count);
                Assert.Equal(3, app.TargetFolder.Folders.Count);

                vm.FolderRemoveCommand.Execute(new PathViewModel(new Path("BB"), false));

                Assert.Equal(2, vm.Folders.Count);
                Assert.Equal(2, app.TargetFolder.Folders.Count);

                Assert.Equal("AA", vm.Folders[0].Path.Value);
                Assert.Equal("CC", vm.Folders[1].Path.Value);
                Assert.Equal("AA", app.TargetFolder.Folders[0].Value);
                Assert.Equal("CC", app.TargetFolder.Folders[1].Value);
            }

            App.Destory();
        }
    }
}