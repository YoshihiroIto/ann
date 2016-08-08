using System;
using System.IO;
using System.Linq;
using Ann.Foundation;
using Ann.Foundation.Exception;
using Xunit;

namespace Ann.Core.Test
{
    public class AppTestFixture : IDisposable
    {
        public AppTestFixture()
        {
            TestHelper.SetEntryAssembly();

            DeleteTestConfigs();
            App.Clean();
            App.RemoveIndexFile();
        }

        public void Dispose()
        {
            App.Clean();
            App.RemoveIndexFile();
            DeleteTestConfigs();
        }

        public static void DeleteTestConfigs()
        {
            var categories = Enum.GetValues(typeof(ConfigHelper.Category)).Cast<ConfigHelper.Category>();

            foreach (var category in categories)
            {
                var path = ConfigHelper.MakeFilePath(category, Constants.ConfigDirPath);

                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }

    public class AppTest : IClassFixture<AppTestFixture>, IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        // ReSharper disable once UnusedParameter.Local
        public AppTest(AppTestFixture f)
        {
            App.Clean();
            App.RemoveIndexFile();
        }

        public void Dispose()
        {
            _context?.Dispose();
            App.Clean();
            App.RemoveIndexFile();
            AppTestFixture.DeleteTestConfigs();
        }

        [Fact]
        public void Basic()
        {
            App.Initialize();

            Assert.NotNull(App.Instance);

            App.Destory();
        }

        [Fact]
        public void NestingInitialize()
        {
            App.Initialize();

            Assert.Throws<NestingException>(() =>
                App.Initialize());
        }

        [Fact]
        public void NestingDestory()
        {
            Assert.Throws<NestingException>(() =>
                App.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            Assert.Throws<UninitializedException>(() =>
                App.Instance.AutoUpdateRemainingSeconds);
        }

        [Fact]
        public void PriorityFile()
        {
            App.Initialize();

            Assert.False(App.Instance.IsPriorityFile("AAA"));

            App.Instance.AddPriorityFile("AAA");
            Assert.True(App.Instance.IsPriorityFile("AAA"));
            App.Instance.AddPriorityFile("AAA");
            Assert.True(App.Instance.IsPriorityFile("AAA"));

            App.Instance.RemovePriorityFile("AAA");
            Assert.False(App.Instance.IsPriorityFile("AAA"));
            App.Instance.RemovePriorityFile("AAA");
            Assert.False(App.Instance.IsPriorityFile("AAA"));

            App.Destory();
        }

        [Fact]
        public void TagetFolders()
        {
            App.Initialize();

            Assert.Equal(5, App.Instance.TagetFolders.Count());

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

            Assert.Equal(0, App.Instance.TagetFolders.Count());
            
            App.Destory();
        }

        [Fact]
        public void OpenIndexAsync()
        {
            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

            App.Instance.OpenIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.NotFound, App.Instance.IndexOpeningResult);

            App.Destory();
        }

        [Fact]
        public void ReopenIndexAsync()
        {
            {
                App.Initialize();

                App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

                App.Instance.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.NotFound, App.Instance.IndexOpeningResult);

                App.Instance.UpdateIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, App.Instance.IndexOpeningResult);

                App.Destory();
            }

            {
                App.Initialize();

                App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

                App.Instance.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, App.Instance.IndexOpeningResult);

                App.Destory();
            }
        }

        [Fact]
        public void UpdateIndexAsync()
        {
            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

            App.Instance.UpdateIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.Ok, App.Instance.IndexOpeningResult);

            App.Destory();
        }

        [Fact]
        public void FindExecutableUnit()
        {
            App.Initialize();

            App.Instance.Config.TargetFolder.Folders.Add(new Path(_context.RootPath));

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;

            _context.CreateFiles(
                "aa.exe",
                "aaa.exe",
                "aaaa.exe");

            App.Instance.UpdateIndexAsync().Wait();

            var f1 = App.Instance.FindExecutableUnit("aaa").ToArray();
            Assert.Equal(2, f1.Length);
            Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f1[0].Path));
            Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f1[1].Path));

            App.Instance.AddPriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

            var f2 = App.Instance.FindExecutableUnit("aaa").ToArray();
            Assert.Equal(2, f2.Length);
            Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f2[0].Path));
            Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f2[1].Path));

            App.Instance.RemovePriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

            App.Destory();
        }

        [Fact]
        public void Mru()
        {
            App.Initialize();

            App.Instance.SaveMru();

            App.Destory();
        }

        [Fact]
        public void AutoUpdater()
        {
            App.Initialize();

            Assert.Equal(0, App.Instance.AutoUpdateRemainingSeconds);
            Assert.Equal(App.AutoUpdateStates.Wait, App.Instance.AutoUpdateState);

            Assert.False(App.Instance.IsEnableAutoUpdater);
            App.Instance.IsEnableAutoUpdater = false;
            Assert.False(App.Instance.IsEnableAutoUpdater);
            App.Instance.IsEnableAutoUpdater = true;
            Assert.True(App.Instance.IsEnableAutoUpdater);

            App.Destory();
        }
    }
}