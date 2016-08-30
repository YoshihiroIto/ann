using Xunit;
using System.Net.NetworkInformation;

namespace Ann.Foundation.Test
{
    public class GoogleSuggestServiceTest
    {
        [Fact]
        public void Basic()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var results = GoogleSuggestService.SuggestAsync("c#", "ja").Result;

            Assert.Equal(10, results.Length);
            Assert.Equal("c#", results[0]);

            foreach (var r in results)
                Assert.True(r.StartsWith("c#"));
        }
    }
}
