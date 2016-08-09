using System.Globalization;
using Ann.Core;
using Ann.SettingWindow.SettingPage.General;
using Xunit;
using TestHelper = Ann.Core.TestHelper;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class GeneralViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

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

        [Theory]
        [InlineData("日本語", "ja")]
        [InlineData("English","en")]
        public void SelectedCulture(string caption, string cultureName)
        {
            TestHelper.CleanTestEnv();

            VersionUpdater.Initialize();
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);

            var app = new Core.Config.App();
            using (var vm = new GeneralViewModel(app))
            {
                Assert.Equal(cultureName, vm.SelectedCulture.Value.CultureName);
                Assert.Equal(caption, vm.SelectedCulture.Value.Caption);
            }

            VersionUpdater.Destory();
        }

        [Fact]
        public void SelectableCulture()
        {
            TestHelper.CleanTestEnv();

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