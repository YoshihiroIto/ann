using Xunit;

namespace Ann.Foundation.Test
{
    public class EvaluatorTest
    {
        [Fact]
        public void Basic()
        {
            var e = new Evaluator();

            Assert.Equal("6", e.Eval("1+2+3"));
            Assert.Equal("0.5", e.Eval("1/2"));
        }
    }
}
