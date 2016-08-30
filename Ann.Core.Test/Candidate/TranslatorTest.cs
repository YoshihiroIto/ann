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
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public TranslatorTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Theory]
        [InlineData("Water", "水", TranslateService.LanguageCodes.en,TranslateService.LanguageCodes.ja)]
        [InlineData("水","Water",TranslateService.LanguageCodes.ja,  TranslateService.LanguageCodes.en)]
        public void Basic(string fromWord, string toWord, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var authenticationFile = "../../../TestData/TranslateServiceTest.yaml";

            if (File.Exists(authenticationFile) == false)
                return;

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            {
                AuthenticationFile auth;
                using (var reader = new StringReader(File.ReadAllText(authenticationFile)))
                    auth = new Deserializer(ignoreUnmatched: true).Deserialize<AuthenticationFile>(reader);

                var s = new Translator(
                    auth.MicrosoftTranslatorClientId,
                    auth.MicrosoftTranslatorClientSecret,
                    languagesService);

                var r = s.TranslateAsync(fromWord, from, to).Result as ICandidate;
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