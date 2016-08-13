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

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            using (new App())
            {
            }
        }

        [Fact]
        public void PriorityFile()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
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
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                Assert.Equal(6, app.TagetFolders.Count());

                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                Assert.Equal(0, app.TagetFolders.Count());
            }
        }

        [Fact]
        public void OpenIndexAsync()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                Assert.Equal(IndexOpeningResults.NotFound, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void ReopenIndexAsync()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.NotFound, app.IndexOpeningResult);

                app.UpdateIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void UpdateIndexAsync()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.UpdateIndexAsync().Wait();

                Assert.Equal(IndexOpeningResults.Ok, app.IndexOpeningResult);
            }
        }

        [Fact]
        public void FindExecutableUnit()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.Folders.Add(new Path(_context.RootPath));

                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                _context.CreateFiles(
                    "aa.exe",
                    "aaa.exe",
                    "aaaa.exe");

                app.UpdateIndexAsync().Wait();

                using (var e1 = new ManualResetEventSlim())
                    // ReSharper disable once AccessToDisposedClosure
                using (app.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
                {
                    app.Find("aaa", 10);
                    e1.Wait();
                }

                var f1 = app.Candidates.ToArray();
                Assert.Equal(2, f1.Length);
                Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f1[0].Path));
                Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f1[1].Path));

                app.AddPriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

                using (var e1 = new ManualResetEventSlim())
                    // ReSharper disable once AccessToDisposedClosure
                using (app.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
                {
                    app.Find("aaa", 10);
                    e1.Wait();
                }

                var f2 = app.Candidates.ToArray();
                Assert.Equal(2, f2.Length);
                Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f2[0].Path));
                Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f2[1].Path));

                app.RemovePriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));
            }
        }

        [Fact]
        public void Mru()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.SaveMru();
            }
        }

        [Fact]
        public void AutoUpdater()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
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