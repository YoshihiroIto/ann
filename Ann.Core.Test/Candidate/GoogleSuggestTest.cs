using System;
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

                Assert.Equal(10, results.Length);
                Assert.Equal("c#", (results[0] as ICandidate).Name);

                foreach (var r in results)
                    Assert.True(((ICandidate) r).Name.StartsWith("c#"));
            }
        }
    }
}