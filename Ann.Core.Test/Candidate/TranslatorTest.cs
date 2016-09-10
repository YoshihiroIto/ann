using System;
using System.IO;
using System.Net.NetworkInformation;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;
using YamlDotNet.Serialization;

namespace Ann.Core.Test.Candidate
{
    public class TranslatorTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Theory]
        [InlineData("Water", "水", TranslateService.LanguageCodes.en,TranslateService.LanguageCodes.ja)]
        [InlineData("水","Water",TranslateService.LanguageCodes.ja,  TranslateService.LanguageCodes.en)]
        public async void Basic(string fromWord, string toWord, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var authenticationFile = "../../../TestData/TranslateServiceTest.yaml";

            if (File.Exists(authenticationFile) == false)
                return;

            var languagesService = _context.GetInstance<LanguagesService>();
            {
                AuthenticationFile auth;
                using (var reader = new StringReader(File.ReadAllText(authenticationFile)))
                    auth = new DeserializerBuilder().IgnoreUnmatchedProperties().Build()
                        .Deserialize<AuthenticationFile>(reader);

                var s = new Translator(
                    auth.MicrosoftTranslatorClientId,
                    auth.MicrosoftTranslatorClientSecret,
                    languagesService);

                var r = await s.TranslateAsync(fromWord, from, to) as ICandidate;
                Assert.Equal(toWord, r.Name);
            }
        }

        public class AuthenticationFile
        {
            public string MicrosoftTranslatorClientId { get; set; }
            public string MicrosoftTranslatorClientSecret { get; set; }
        }
    }
}