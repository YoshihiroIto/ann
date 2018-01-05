using System;
using Ann.Core.Candidate;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class CalculatorTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var c = new Calculator();

                var r = c.Calculate("4*5", languagesService);

                Assert.Equal("20", r.Name);
            }
        }

        [Fact]
        public void Error()
        {
            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var c = new Calculator();

                var r = c.Calculate("(123", languagesService);

                Assert.Null(r);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        public void Null(string input)
        {
            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var c = new Calculator();

                var actual = c.Calculate(input, languagesService);

                Assert.Null(actual);
            }
        }
    }
}