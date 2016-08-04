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

            Assert.Equal(o.Auther, "A");
            Assert.Equal(o.Name, "B");
            Assert.Equal(o.Summry, "C");
            Assert.Equal(o.Url, "D");
        }
    }
}
