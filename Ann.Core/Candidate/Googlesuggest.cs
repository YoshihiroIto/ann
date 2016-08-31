using System.Collections.Generic;
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

        public async Task<IEnumerable<GoogleSearchResult>> SuggestAsync(string input, string language)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var result = await GoogleSuggestService.SuggestAsync(input, language);

            return result?.Select(r => new GoogleSearchResult(r, _languagesService, StringTags.GoogleSuggest));
        }
    }
}