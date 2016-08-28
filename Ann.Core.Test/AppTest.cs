using System;
using System.Linq;
using System.Threading;
using Ann.Foundation;
using Reactive.Bindings.Extensions;
using Xunit;

namespace Ann.Core.Test
{
    public class AppTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public AppTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _context.Dispose();
            _config.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                Assert.True(app.IsEnableActivateHotKey);
                app.IsEnableActivateHotKey = false;
                Assert.False(app.IsEnableActivateHotKey);

                Assert.False(app.IsRestartRequested);
            }
        }

        [Fact]
        public void PriorityFile()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
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
        }

        [Fact]
        public void TagetFolders()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                Assert.Equal(6, app.TagetFolders.Count());

                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                Assert.Equal(0, app.TagetFolders.Count());
            }
        }

        [Fact]
        public void OpenIndexAsync()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                Assert.Equal(IndexOpeningResults.NotFound, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void ReopenIndexAsync()
        {
            {
                var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
                using (var app = new App(configHolder, languagesService))
                {
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
            }

            {
                var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
                using (var app = new App(configHolder, languagesService))
                {
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
        }

        [Fact]
        public void UpdateIndexAsync()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.UpdateIndexAsync().Wait();

                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void FindExecutableFile()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                configHolder.Config.TargetFolder.Folders.Add(new Path(_context.RootPath));

                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                _context.CreateFiles(
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

                app.AddPriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

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

                app.RemovePriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));
            }
        }

        [Fact]
        public void AutoUpdater()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                Assert.Equal(0, app.AutoUpdateRemainingSeconds);
                Assert.Equal(App.AutoUpdateStates.Wait, app.AutoUpdateState);

                Assert.False(app.IsEnableAutoUpdater);
                app.IsEnableAutoUpdater = false;
                Assert.False(app.IsEnableAutoUpdater);
                app.IsEnableAutoUpdater = true;
                Assert.True(app.IsEnableAutoUpdater);
            }
        }
    }
}