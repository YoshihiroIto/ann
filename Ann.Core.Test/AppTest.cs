using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Ann.Foundation;
using Reactive.Bindings.Extensions;
using Xunit;

namespace Ann.Core.Test
{
    public class AppTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var app = _context.GetInstance<App>();

            Assert.True(app.IsEnableActivateHotKey);
            app.IsEnableActivateHotKey = false;
            Assert.False(app.IsEnableActivateHotKey);

            Assert.False(app.IsRestartRequested);
        }

        [Fact]
        public void PriorityFile()
        {
            var app = _context.GetInstance<App>();

            Assert.False(app.IsPriorityFile("AAA"));

            app.AddPriorityFile("AAA");
            Assert.True(app.IsPriorityFile("AAA"));
            app.AddPriorityFile("AAA");
            Assert.True(app.IsPriorityFile("AAA"));

            app.RemovePriorityFile("AAA");
            Assert.False(app.IsPriorityFile("AAA"));
            app.RemovePriorityFile("AAA");
            Assert.False(app.IsPriorityFile("AAA"));
        }

        [Fact]
        public void TagetFolders()
        {
            var app = _context.GetInstance<App>();
            var configHolder = _context.GetInstance<ConfigHolder>();

            Assert.Equal(6, app.TagetFolders.Count());

            configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
            configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            Assert.Empty(app.TagetFolders);
        }

        [Fact]
        public void OpenIndexAsync()
        {
            var app = _context.GetInstance<App>();
            var configHolder = _context.GetInstance<ConfigHolder>();

            configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
            configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            app.OpenIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.NotFound, app.IndexOpeningResult);
        }

        [Fact]
        public void ReopenIndexAsync()
        {
            {
                var app = _context.GetInstance<App>();
                var configHolder = _context.GetInstance<ConfigHolder>();

                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.NotFound, app.IndexOpeningResult);

                app.UpdateIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }

            {
                var app = _context.GetInstance<App>();
                var configHolder = _context.GetInstance<ConfigHolder>();

                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void UpdateIndexAsync()
        {
            var app = _context.GetInstance<App>();
            var configHolder = _context.GetInstance<ConfigHolder>();

            configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
            configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            app.UpdateIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
        }

        [Fact]
        public void Find_Null()
        {
            var app = _context.GetInstance<App>();

            var c = app.Candidates.ToArray();

            app.Find(null);

            Assert.Equal(c, app.Candidates);
        }

        [Fact]
        public void Find_Space()
        {
            var app = _context.GetInstance<App>();

            app.Find("          ");

            Assert.Empty(app.Candidates);
        }

        [Fact]
        public void Find_Calculate()
        {
            var app = _context.GetInstance<App>();

            using (var e1 = new ManualResetEventSlim())
                // ReSharper disable once AccessToDisposedClosure
            using (app.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
            {
                app.Find("12+12");
                e1.Wait();
            }

            Assert.Single(app.Candidates);
            Assert.Equal("24", app.Candidates.FirstOrDefault()?.Name);
        }

        [Fact]
        public void Find_ExecutableFile()
        {
            var app = _context.GetInstance<App>();
            var configHolder = _context.GetInstance<ConfigHolder>();

            using (var dataFs = new DisposableFileSystem())
            {
                configHolder.Config.TargetFolder.Folders.Add(new Path(dataFs.RootPath));

                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                dataFs.CreateFiles(
                    "aa.exe",
                    "aaa.exe",
                    "aaaa.exe");

                app.UpdateIndexAsync().Wait();

                using (var e1 = new ManualResetEventSlim())
                    // ReSharper disable once AccessToDisposedClosure
                using (app.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
                {
                    app.Find("aaa");
                    e1.Wait();
                }

                var f1 = app.Candidates.ToArray();
                Assert.Equal(2, f1.Length);
                Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f1[0].Comment));
                Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f1[1].Comment));

                app.AddPriorityFile(System.IO.Path.Combine(dataFs.RootPath, "aaaa.exe"));

                using (var e1 = new ManualResetEventSlim())
                    // ReSharper disable once AccessToDisposedClosure
                using (app.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
                {
                    app.Find("aaa");
                    e1.Wait();
                }

                var f2 = app.Candidates.ToArray();
                Assert.Equal(2, f2.Length);
                Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f2[0].Comment));
                Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f2[1].Comment));

                app.RemovePriorityFile(System.IO.Path.Combine(dataFs.RootPath, "aaaa.exe"));
            }
        }

        [Fact]
        public void AutoUpdater()
        {
            var app = _context.GetInstance<App>();

            Assert.Equal(0, app.AutoUpdateRemainingSeconds);
            Assert.Equal(App.AutoUpdateStates.Wait, app.AutoUpdateState);

            Assert.False(app.IsEnableAutoUpdater);
            app.IsEnableAutoUpdater = false;
            Assert.False(app.IsEnableAutoUpdater);
            app.IsEnableAutoUpdater = true;
            Assert.True(app.IsEnableAutoUpdater);
        }

        [Fact]
        public void NoticeMessages()
        {
            var app = _context.GetInstance<App>();
            var messages = new List<StringTags> {StringTags.AllFiles};

            app.Notification += (s, e) =>
            {
                // ReSharper disable once AccessToDisposedClosure
                Assert.Same(app, s);
                Assert.Equal(messages, e.Messages);
            };

            app.NoticeMessages(messages);
        }

        [Fact]
        public void ExecutableFileDataBaseIconCacheSize()
        {
            var app = _context.GetInstance<App>();

            Assert.Equal(0, app.ExecutableFileDataBaseIconCacheSize);
            app.ExecutableFileDataBaseIconCacheSize = 10;
            Assert.Equal(10, app.ExecutableFileDataBaseIconCacheSize);
        }

        [Fact]
        public async void RunAsync()
        {
            var app = _context.GetInstance<App>();
            var i = await app.RunAsync("notepad", false);

            Assert.True(i);

            foreach (var p in Process.GetProcessesByName("notepad"))
                p.Kill();
        }
    }
}