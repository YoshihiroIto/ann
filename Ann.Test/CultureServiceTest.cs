using System.Globalization;
using System.Threading;
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
            var expected = default(string);
            var actual = default(string);

            var th = new Thread(() =>
            {
                expected = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                actual = CultureService.Instance.CultureName;

                CultureService.Instance.CultureName = "en";
            });
            th.Start();
            th.Join();

            Assert.Equal(expected, actual);
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