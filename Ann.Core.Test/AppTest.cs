﻿using System;
using System.Linq;
using System.Threading;
using Ann.Foundation;
using Ann.Foundation.Exception;
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

            App.Initialize();

            Assert.NotNull(App.Instance);

            App.Destory();
        }

        [Fact]
        public void NestingInitialize()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            Assert.Throws<NestingException>(() =>
                App.Initialize());
        }

        [Fact]
        public void NestingDestory()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<NestingException>(() =>
                App.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<UninitializedException>(() =>
                App.Instance.AutoUpdateRemainingSeconds);
        }

        [Fact]
        public void PriorityFile()
        {
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

            App.Initialize();

            Assert.Equal(6, App.Instance.TagetFolders.Count());

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            Assert.Equal(0, App.Instance.TagetFolders.Count());

            App.Destory();
        }

        [Fact]
        public void OpenIndexAsync()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            App.Instance.OpenIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.NotFound, App.Instance.IndexOpeningResult);

            App.Destory();
        }

        [Fact]
        public void ReopenIndexAsync()
        {
            TestHelper.CleanTestEnv();

            {
                App.Initialize();

                App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

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
                App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                App.Instance.OpenIndexAsync().Wait();
                Assert.Equal(IndexOpeningResults.Ok, App.Instance.IndexOpeningResult);

                App.Destory();
            }
        }

        [Fact]
        public void UpdateIndexAsync()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            App.Instance.UpdateIndexAsync().Wait();

            Assert.Equal(IndexOpeningResults.Ok, App.Instance.IndexOpeningResult);

            App.Destory();
        }

        [Fact]
        public void FindExecutableUnit()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            App.Instance.Config.TargetFolder.Folders.Add(new Path(_context.RootPath));

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            _context.CreateFiles(
                "aa.exe",
                "aaa.exe",
                "aaaa.exe");

            App.Instance.UpdateIndexAsync().Wait();

            using (var e1 = new ManualResetEventSlim())
                // ReSharper disable once AccessToDisposedClosure
            using (App.Instance.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
            {
                App.Instance.Find("aaa", 10);
                e1.Wait();
            }

            var f1 = App.Instance.Candidates.ToArray();
            Assert.Equal(2, f1.Length);
            Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f1[0].Path));
            Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f1[1].Path));

            App.Instance.AddPriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

            using (var e1 = new ManualResetEventSlim())
                // ReSharper disable once AccessToDisposedClosure
            using (App.Instance.ObserveProperty(x => x.Candidates, false).Subscribe(_ => e1.Set()))
            {
                App.Instance.Find("aaa", 10);
                e1.Wait();
            }

            var f2 = App.Instance.Candidates.ToArray();
            Assert.Equal(2, f2.Length);
            Assert.Equal("aaaa.exe", System.IO.Path.GetFileName(f2[0].Path));
            Assert.Equal("aaa.exe", System.IO.Path.GetFileName(f2[1].Path));

            App.Instance.RemovePriorityFile(System.IO.Path.Combine(_context.RootPath, "aaaa.exe"));

            App.Destory();
        }

        [Fact]
        public void Mru()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            App.Instance.SaveMru();

            App.Destory();
        }

        [Fact]
        public void AutoUpdater()
        {
            TestHelper.CleanTestEnv();

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