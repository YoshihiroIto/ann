using System;
using System.Globalization;
using System.Threading;
using Ann.Core;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.General
{
    public class GeneralViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var app = _context.GetInstance<Core.Config.App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (new GeneralViewModel(app, versionUpdater))
            {
            }
        }

        [Fact]
        public void MaxCandidateLinesCount()
        {
            var app = _context.GetInstance<Core.Config.App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var vm = new GeneralViewModel(app, versionUpdater))
            {
                Assert.Equal(10, vm.MaxCandidateLinesCount.Value);

                vm.MaxCandidateLinesCount.Value = 7;
                Assert.Equal(7, vm.MaxCandidateLinesCount.Value);
                Assert.Equal(7, app.MaxCandidateLinesCount);

                app.MaxCandidateLinesCount = 6;
                Assert.Equal(6, vm.MaxCandidateLinesCount.Value);
            }
        }

        [Fact]
        public void SelectedCulture_ja()
        {
            var cultureName = default(string);
            var caption = default(string);

            var th = new Thread(() =>
            {
                CultureInfo.CurrentUICulture = new CultureInfo("ja");

            var app = _context.GetInstance<Core.Config.App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

                using (var vm = new GeneralViewModel(app, versionUpdater))
                {
                    cultureName = vm.SelectedCulture.Value.CultureName;
                    caption = vm.SelectedCulture.Value.Caption;
                }
            });
            th.Start();
            th.Join();

            Assert.Equal("ja", cultureName);
            Assert.Equal("日本語", caption);
        }

        [Theory]
        [InlineData("日本語", "ja")]
        [InlineData("English","en")]
        public void SelectedCulture(string caption, string cultureName)
        {
            var expectedCultureName = default(string);
            var expectedCaption = default(string);

            var th = new Thread(() =>
            {
                CultureInfo.CurrentUICulture = new CultureInfo(cultureName);

            var app = _context.GetInstance<Core.Config.App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

                using (var vm = new GeneralViewModel(app, versionUpdater))
                {
                    expectedCultureName = vm.SelectedCulture.Value.CultureName;
                    expectedCaption = vm.SelectedCulture.Value.Caption;
                }
            });
            th.Start();
            th.Join();

            Assert.Equal(cultureName, expectedCultureName);
            Assert.Equal(caption, expectedCaption);
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