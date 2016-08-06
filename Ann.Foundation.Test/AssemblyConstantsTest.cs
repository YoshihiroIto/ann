using System.Text.RegularExpressions;
using Xunit;

namespace Ann.Foundation.Test
{
    public class AssemblyConstantsTest
    {
        [Fact]
        public void Version()
        {
            TestHelper.SetEntryAssembly();

            var r = new Regex(@"\d+.\d+.\d+.\d+");
            Assert.True(r.IsMatch(AssemblyConstants.Version));
        }
    }
}