﻿using System;
using System.Globalization;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class GeneralTestFixture : IDisposable
    {
        public GeneralTestFixture()
        {
            TestHelper.SetEntryAssembly();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class GeneralTest : IClassFixture<GeneralTestFixture>, IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public GeneralTest(GeneralTestFixture f)
        {
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }

        [Fact]
        public void Basic()
        {
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (new GeneralViewModel(app))
            {
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void MaxCandidateLinesCount()
        {
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new GeneralViewModel(app))
            {
                Assert.Equal(8, vm.MaxCandidateLinesCount.Value);

                vm.MaxCandidateLinesCount.Value = 7;
                Assert.Equal(7, vm.MaxCandidateLinesCount.Value);
                Assert.Equal(7, app.MaxCandidateLinesCount);

                app.MaxCandidateLinesCount = 6;
                Assert.Equal(6, vm.MaxCandidateLinesCount.Value);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void SelectedCulture_ja()
        {
            VersionUpdater.Initialize();
            CultureInfo.CurrentUICulture = new CultureInfo("ja");

            var app = new Core.Config.App();
            using (var vm = new GeneralViewModel(app))
            {
                Assert.Equal("ja", vm.SelectedCulture.Value.CultureName);
                Assert.Equal("日本語", vm.SelectedCulture.Value.Caption);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void SelectedCulture_en()
        {
            VersionUpdater.Initialize();
            CultureInfo.CurrentUICulture = new CultureInfo("en");

            var app = new Core.Config.App();
            using (var vm = new GeneralViewModel(app))
            {
                Assert.Equal("en", vm.SelectedCulture.Value.CultureName);
                Assert.Equal("English", vm.SelectedCulture.Value.Caption);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void SelectableCulture()
        {
            var sc = new SelectableCulture();

            Assert.Null(sc.CultureName);
            Assert.Null(sc.Caption);

            sc.CultureName = "AA";
            sc.Caption = "BB";

            Assert.Equal("AA", sc.CultureName);
            Assert.Equal("BB", sc.Caption);
        }
    }
}