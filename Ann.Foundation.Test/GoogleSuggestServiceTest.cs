using System.Linq;
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

            using (var s = new GoogleSuggestService())
            {
                var sr = await s.SuggestAsync("c#", "ja");

                var results = sr.ToArray();

                Assert.Equal(10, results.Length);
                Assert.Equal("c#", results[0]);

                foreach (var r in results)
                    Assert.True(r.StartsWith("c#"));
            }
        }
    }
}