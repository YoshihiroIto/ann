using Xunit;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Ann.Foundation.Test
{
    public class GoogleSuggestServiceTest
    {
        [Fact]
        public async Task Basic()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var results = await GoogleSuggestService.SuggestAsync("c#", "ja");

            Assert.Equal(10, results.Length);
            Assert.Equal("c#", results[0]);

            foreach (var r in results)
                Assert.True(r.StartsWith("c#"));
        }
    }
}
