using System.IO;
using Xunit;
using YamlDotNet.Serialization;

namespace Ann.Foundation.Test
{
    public class TranslateServiceTest
    {
        [Theory]
        [InlineData("a", null)]
        [InlineData(null, "a")]
        [InlineData(null, null)]
        public void Null(string clientId, string clientSecret)
        {
            var s = new TranslateService(clientId, clientSecret);

            var r = s.TranslateAsync(
                "Apple",
                TranslateService.LanguageCodes.en,
                TranslateService.LanguageCodes.ja).Result;

            Assert.Null(r);
        }

        [Fact]
        public void AuthErrro()
        {
            var s = new TranslateService("XXX", "YYY");

            var r = s.TranslateAsync(
                "Apple",
                TranslateService.LanguageCodes.en,
                TranslateService.LanguageCodes.ja).Result;

            Assert.Null(r);
        }

        [Theory]
        [InlineData("Water", "水", TranslateService.LanguageCodes.en,TranslateService.LanguageCodes.ja)]
        [InlineData("水","Water",TranslateService.LanguageCodes.ja,  TranslateService.LanguageCodes.en)]
        public void Basic(string fromWord, string toWord, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            var authenticationFile = "../../TranslateServiceTest.yaml";

            if (File.Exists(authenticationFile) == false)
                return;

            AuthenticationFile auth;
            using (var reader = new StringReader(File.ReadAllText(authenticationFile)))
                auth = new Deserializer(ignoreUnmatched: true).Deserialize<AuthenticationFile>(reader);

            var s = new TranslateService(
                auth.MicrosoftTranslatorClientId, 
                auth.MicrosoftTranslatorClientSecret);

            var r = s.TranslateAsync(fromWord, from, to).Result;
            Assert.Equal(toWord, r);
        }

        [Theory]
        [InlineData("Water", "水", TranslateService.LanguageCodes.ja)]
        [InlineData("水","Water", TranslateService.LanguageCodes.en)]
        public void Auto(string fromWord, string toWord, TranslateService.LanguageCodes to)
        {
            const string authFile = "../../TranslateServiceTest.yaml";

            if (File.Exists(authFile) == false)
                return;

            AuthenticationFile auth;
            using (var reader = new StringReader(File.ReadAllText(authFile)))
                auth = new Deserializer(ignoreUnmatched: true).Deserialize<AuthenticationFile>(reader);

            var s = new TranslateService(
                auth.MicrosoftTranslatorClientId, 
                auth.MicrosoftTranslatorClientSecret);

            var r = s.TranslateAsync(fromWord, TranslateService.LanguageCodes.AutoDetect, to).Result;
            Assert.Equal(toWord, r);
        }

        public class AuthenticationFile
        {
            public string MicrosoftTranslatorClientId { get; set; }
            public string MicrosoftTranslatorClientSecret { get; set; }
        }
    }
}
