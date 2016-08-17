using Xunit;

namespace Ann.Core.Test
{
    public class CultureSummryTest
    {
        public CultureSummryTest()
        {
            TestHelper.CleanTestEnv();
        }
    
        [Fact]
        public void Basic()
        {
            var c = new CultureSummry
            {
                Caption = "A",
                CultureName = "B"
            };

            Assert.Equal("A", c.Caption);
            Assert.Equal("B", c.CultureName);
        }

        [Fact]
        public void DefaultCtor()
        {
            var c = new CultureSummry();

            Assert.Null(c.Caption);
            Assert.Null(c.CultureName);
        }
    }
}