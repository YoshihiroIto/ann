using Ann.Core.Candidate;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class CalculationResultTest
    {
        [Fact]
        public void Basic()
        {
            var r = new CalculationResult("ABC");

            // ReSharper disable once IsExpressionAlwaysTrue
            Assert.True(r is ICandidate);
        }

        [Fact]
        public void Interface()
        {
            var i = new CalculationResult("ABC") as ICandidate;

            Assert.Equal("Calculator", i.Comment);
            Assert.Equal("ABC", i.Name);
            Assert.Null(i.RunCommand);
            Assert.Null(i.SubCommands);
            Assert.False(i.CanSetPriority);
        }
    }
}