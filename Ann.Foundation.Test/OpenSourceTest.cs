using Xunit;

namespace Ann.Foundation.Test
{
    public class OpenSourceTest
    {
        [Fact]
        public void Simple()
        {
            var o = new OpenSource
            {
                Auther = "A",
                Name = "B",
                Summry = "C",
                Url = "D"
            };

            Assert.Equal("A", o.Auther);
            Assert.Equal("B", o.Name);
            Assert.Equal("C", o.Summry);
            Assert.Equal("D", o.Url);
        }
    }
}
