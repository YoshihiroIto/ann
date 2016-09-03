using System;
using System.Linq;
using System.Net.NetworkInformation;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class GoogleSuggestTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }


        [Fact]
        public async void Basic()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var g = new GoogleSuggest(languagesService);

                var results = await g.SuggestAsync("c#", "ja");

                var a = results.ToArray();

                Assert.Equal(10, a.Length);
                Assert.Equal("c#", (a[0] as ICandidate).Name);

                foreach (var r in a)
                    Assert.True(((ICandidate) r).Name.StartsWith("c#"));
            }
        }
    }
}