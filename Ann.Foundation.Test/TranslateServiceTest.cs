using System.IO;
using Xunit;
using YamlDotNet.Serialization;
using System.Net.NetworkInformation;

namespace Ann.Foundation.Test
{
    public class TranslateServiceTest
    {
        [Theory]
        [InlineData("a", null)]
        [InlineData(null, "a")]
        [InlineData(null, null)]
        [InlineData("a", "a")]
        public async void Error(string clientId, string clientSecret)
        {
            var s = new TranslateService(clientId, clientSecret);

            var r = await s.TranslateAsync(
                "Apple",
                TranslateService.LanguageCodes.en,
                TranslateService.LanguageCodes.ja);

            Assert.Null(r);
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

            AuthenticationFile auth;
            using (var reader = new StringReader(File.ReadAllText(authenticationFile)))
                auth = new Deserializer(ignoreUnmatched: true).Deserialize<AuthenticationFile>(reader);

            var s = new TranslateService(
                auth.MicrosoftTranslatorClientId, 
                auth.MicrosoftTranslatorClientSecret);

            var r = await s.TranslateAsync(fromWord, from, to);
            Assert.Equal(toWord, r);
        }

        [Theory]
        [InlineData("Water", "水", TranslateService.LanguageCodes.ja)]
        [InlineData("水","Water", TranslateService.LanguageCodes.en)]
        public async void Auto(string fromWord, string toWord, TranslateService.LanguageCodes to)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            const string authFile = "../../TranslateServiceTest.yaml";

            if (File.Exists(authFile) == false)
                return;

            AuthenticationFile auth;
            using (var reader = new StringReader(File.ReadAllText(authFile)))
                auth = new Deserializer(ignoreUnmatched: true).Deserialize<AuthenticationFile>(reader);

            var s = new TranslateService(
                auth.MicrosoftTranslatorClientId, 
                auth.MicrosoftTranslatorClientSecret);

            var r = await s.TranslateAsync(fromWord, TranslateService.LanguageCodes.AutoDetect, to);
            Assert.Equal(toWord, r);
        }

        public class AuthenticationFile
        {
            public string MicrosoftTranslatorClientId { get; set; }
            public string MicrosoftTranslatorClientSecret { get; set; }
        }
    }
}
