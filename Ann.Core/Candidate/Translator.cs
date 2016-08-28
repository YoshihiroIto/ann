using System.Diagnostics;
using System.Threading.Tasks;
using Ann.Foundation;

namespace Ann.Core.Candidate
{
    public class Translator
    {
        private readonly App _app;
        private readonly TranslateService _service;

        public Translator(App app, string clientId, string clientSecret)
        {
            Debug.Assert(app != null);
            _app = app;
            _service = new TranslateService(clientId, clientSecret);
        }

        public async Task<TranslateResult> TranslateAsync(string input, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var r = await _service.TranslateAsync(input, from, to);

            if (r == null)
                return null;

            return new TranslateResult(r, _app);
        }
    }
}