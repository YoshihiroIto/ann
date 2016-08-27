using Ann.Core.Candidate;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class CalculatorTest
    {
        [Fact]
        public void Basic()
        {
            var c = new Calculator();

            var r = c.Calculate("4*5");

            Assert.Equal(r.Name, "20");
        }

        [Fact]
        public void Error()
        {
            var c = new Calculator();

            var r = c.Calculate("(123");

            Assert.Null(r);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        public void Null(string input)
        {
            var c = new Calculator();

            var actual = c.Calculate(input);

            Assert.Null(actual);
        }
    }
}