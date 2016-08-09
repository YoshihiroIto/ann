using Xunit;

namespace Ann.Core.Test
{
    public class CultureSummryTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var c = new CultureSummry
            {
                Caption = "A",
                CultureName = "B"
            };

            Assert.Equal("A", c.Caption);
            Assert.Equal("B", c.CultureName);
        }
    }
}