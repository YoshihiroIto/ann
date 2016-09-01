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
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public GoogleSuggestTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public async void Basic()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
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