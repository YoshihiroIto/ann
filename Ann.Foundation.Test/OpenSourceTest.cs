using Xunit;

namespace Ann.Foundation.Test
{
    public class OpenSourceTest
    {
        [Fact]
        public void Basic()
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

        [Fact]
        public void DefaultCtor()
        {
            var o = new OpenSource();

            Assert.Null(o.Auther);
            Assert.Null(o.Name);
            Assert.Null(o.Summry);
            Assert.Null(o.Url);
        }
    }
}
