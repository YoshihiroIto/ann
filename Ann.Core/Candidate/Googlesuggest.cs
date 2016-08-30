using System.Linq;
using System.Threading.Tasks;
using Ann.Foundation;

namespace Ann.Core.Candidate
{
    public class GoogleSuggest
    {
        private readonly LanguagesService _languagesService;

        public GoogleSuggest(LanguagesService languagesService)
        {
            _languagesService = languagesService;
        }

        public async Task<GoogleSuggestResult[]> SuggestAsync(string input, string language)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var result = await GoogleSuggestService.SuggestAsync(input, language);

            return result?.Select(r => new GoogleSuggestResult(r, _languagesService)).ToArray();
        }
    }
}