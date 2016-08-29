using System.Diagnostics;
using System.Threading.Tasks;
using Ann.Foundation;

namespace Ann.Core.Candidate
{
    public class Translator
    {
        private readonly LanguagesService _languagesService;
        private readonly TranslateService _service;

        public Translator(string clientId, string clientSecret, LanguagesService languagesService)
        {
            _languagesService = languagesService;
            _service = new TranslateService(clientId, clientSecret);
        }

        public async Task<TranslateResult> TranslateAsync(string input, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var r = await _service.TranslateAsync(input, from, to);

            if (r == null)
                return null;

            return new TranslateResult(r, _languagesService);
        }
    }
}