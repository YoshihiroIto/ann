using System.Globalization;
using Ann.Core;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.General
{
    public class GeneralViewModelTest
    {
        public GeneralViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void Basic()
        {
            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (new GeneralViewModel(app, versionUpdater))
            {
            }
        }

        [Fact]
        public void MaxCandidateLinesCount()
        {
            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new GeneralViewModel(app, versionUpdater))
            {
                Assert.Equal(8, vm.MaxCandidateLinesCount.Value);

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
            CultureInfo.CurrentUICulture = new CultureInfo("ja");

            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new GeneralViewModel(app, versionUpdater))
            {
                Assert.Equal("ja", vm.SelectedCulture.Value.CultureName);
                Assert.Equal("日本語", vm.SelectedCulture.Value.Caption);
            }
        }

        [Theory]
        [InlineData("日本語", "ja")]
        [InlineData("English","en")]
        public void SelectedCulture(string caption, string cultureName)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);

            var app = new Core.Config.App();

            using (var versionUpdater = new VersionUpdater(null))
            using (var vm = new GeneralViewModel(app, versionUpdater))
            {
                Assert.Equal(cultureName, vm.SelectedCulture.Value.CultureName);
                Assert.Equal(caption, vm.SelectedCulture.Value.Caption);
            }
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