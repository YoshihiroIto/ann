using System.Globalization;
using Ann.Core.Config;
using Xunit;

namespace Ann.Test
{
    public class CultureServiceTest
    {
        public CultureServiceTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void CultureName()
        {
            Assert.Equal(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, CultureService.Instance.CultureName);

            CultureService.Instance.CultureName = "en";
        }

        [Fact]
        public void Resources()
        {
            Assert.NotNull(CultureService.Instance.Resources);
        }

        [Fact]
        public void SetConfig()
        {
            CultureService.Instance.SetConfig(new App());

            CultureService.Instance.Destory();
        }
    }
}