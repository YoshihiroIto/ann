using Xunit;

namespace Ann.Foundation.Test
{
    public class EvaluatorTest
    {
        [Fact]
        public void Basic()
        {
            Assert.Equal("6", Evaluator.Eval("1+2+3"));
            Assert.Equal("0.5", Evaluator.Eval("1/2"));
        }
    }
}
