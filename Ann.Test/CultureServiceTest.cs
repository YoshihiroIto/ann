using System.Globalization;
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
    }
}