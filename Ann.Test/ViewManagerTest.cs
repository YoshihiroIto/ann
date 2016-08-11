using System.Globalization;
using Ann.Core.Config;
using Ann.Foundation.Exception;
using Xunit;

namespace Ann.Test
{
    public class CultureServiceTest
    {
        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var config = new App();
            CultureService.Initialize(config);

            CultureService.Destory();
        }


        [Fact]
        public void NestingInitialize()
        {
            TestHelper.CleanTestEnv();

            var config = new App();
            CultureService.Initialize(config);

            Assert.Throws<NestingException>(() =>
                CultureService.Initialize(config));
        }

        [Fact]
        public void NestingDestory()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<NestingException>(() =>
                CultureService.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<UninitializedException>(() =>
                CultureService.Instance.CultureName);
        }

        [Theory]
        [InlineData("ja")]
        [InlineData("en")]
        public void CultureName(string lang)
        {
            TestHelper.CleanTestEnv();

            CultureInfo.CurrentUICulture = new CultureInfo(lang);

            var config = new App();
            CultureService.Initialize(config);

            Assert.Equal(lang, CultureService.Instance.CultureName);

            CultureService.Instance.CultureName = lang;

            CultureService.Destory();
        }

        [Theory]
        [InlineData("ja")]
        [InlineData("en")]
        public void Resources(string lang)
        {
            TestHelper.CleanTestEnv();

            CultureInfo.CurrentUICulture = new CultureInfo(lang);

            var config = new App();
            CultureService.Initialize(config);

            Assert.NotNull(CultureService.Instance.Resources);

            Assert.Equal(lang, Properties.Resources.Culture.TwoLetterISOLanguageName);

            CultureService.Destory();
        }
    }
}