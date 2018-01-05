using System.Text.RegularExpressions;
using Xunit;

namespace Ann.Foundation.Test
{
    public class AssemblyConstantsTest
    {
        [Fact]
        public void Version()
        {
            Assert.Matches(@"\d+.\d+.\d+.\d+", AssemblyConstants.Version);
        }
    }
}