using System.Globalization;
using Xunit;
using Ann.Core.Properties;

namespace Ann.Core.Test
{
    public class ResourcesTest
    {
        [Fact]
        public void Basic()
        {
            Assert.NotNull(Resources.ResourceManager.BaseName);
            Assert.Null(Resources.Culture);

            Resources.Culture = new CultureInfo("ja");
            Assert.NotNull(Resources.Culture);

            Assert.NotNull(Resources.OpenSourceList);
            Assert.NotNull(Resources.OpenSourceListNonNuget);
        }
    }
}