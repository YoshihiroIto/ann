using System.Globalization;
using Ann.Core.Config;
using Xunit;

namespace Ann.Test
{
    public class CultureServiceTest
    {
        [Fact]
        public void CultureName()
        {
            TestHelper.CleanTestEnv();

            Assert.Equal(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, CultureService.Instance.CultureName);

            CultureService.Instance.CultureName = "en";
        }

        [Fact]
        public void Resources()
        {
            TestHelper.CleanTestEnv();

            Assert.NotNull(CultureService.Instance.Resources);
        }

        [Fact]
        public void SetConfig()
        {
            TestHelper.CleanTestEnv();
            
            CultureService.Instance.SetConfig(new App());

            CultureService.Instance.Destory();
        }
    }
}