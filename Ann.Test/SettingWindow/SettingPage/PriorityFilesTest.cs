using System;
using System.Reactive.Concurrency;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Reactive.Bindings;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class PriorityFilesTestFixture : IDisposable
    {
        public PriorityFilesTestFixture()
        {
            TestHelper.SetEntryAssembly();
            App.Clean();
            VersionUpdater.Clean();
            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class PriorityFilesTest : IClassFixture<PriorityFilesTestFixture>,  IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public PriorityFilesTest(PriorityFilesTestFixture f)
        {
            App.Clean();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            App.Clean();
            VersionUpdater.Clean();
        }

        [Fact]
        public void Basic()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (new PriorityFilesViewModel(app))
            {
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void Files()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new PriorityFilesViewModel(app))
            {
                Assert.Equal(0, vm.Files.Count);

                app.PriorityFiles.Add(new Path("AA"));
                Assert.Equal(1, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);

                app.PriorityFiles.Add(new Path("BB"));
                Assert.Equal(2, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);
                Assert.Equal("BB", vm.Files[1].Path.Value);

                app.PriorityFiles.RemoveAt(0);
                Assert.Equal(1, vm.Files.Count);
                Assert.Equal("BB", vm.Files[0].Path.Value);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void FileAddCommand()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new PriorityFilesViewModel(app))
            {
                vm.FileAddCommand.Execute(null);

                Assert.Equal(1, vm.Files.Count);
                Assert.Equal(string.Empty, vm.Files[0].Path.Value);

                Assert.Equal(1, app.PriorityFiles.Count);
                Assert.Equal(string.Empty, app.PriorityFiles[0].Value);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void FileRemoveCommand()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new PriorityFilesViewModel(app))
            {
                Assert.Equal(0, vm.Files.Count);

                app.PriorityFiles.Add(new Path("AA"));
                app.PriorityFiles.Add(new Path("BB"));
                app.PriorityFiles.Add(new Path("CC"));

                vm.FileRemoveCommand.Execute(new PathViewModel(new Path("BB"), false));

                Assert.Equal(2, vm.Files.Count);
                Assert.Equal("AA", vm.Files[0].Path.Value);
                Assert.Equal("CC", vm.Files[1].Path.Value);

                Assert.Equal(2, app.PriorityFiles.Count);
                Assert.Equal("AA", app.PriorityFiles[0].Value);
                Assert.Equal("CC", app.PriorityFiles[1].Value);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void PathFolderSelect()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new PriorityFilesViewModel(app))
            {
                app.PriorityFiles.Add(new Path("AA"));
                vm.Files[0].FolderSelectDialogOpenCommand.Execute(null);
            }

            VersionUpdater.Destory();
            App.Destory();
        }
    }
}