using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Core.Candidate
{
    public class GoogleSuggest : DisposableNotificationObject
    {
        private readonly LanguagesService _languagesService;
        private readonly GoogleSuggestService _service;

        #region IsInConnecting

        private bool _IsInConnecting;

        public bool IsInConnecting
        {
            get { return _IsInConnecting; }
            set { SetProperty(ref _IsInConnecting, value); }
        }

        #endregion

        public GoogleSuggest(LanguagesService languagesService)
        {
            _languagesService = languagesService;
            _service = new GoogleSuggestService().AddTo(CompositeDisposable);
        }

        public void CancelSuggest()
        {
            _service.CancelSuggest();
        }

        public async Task<IEnumerable<GoogleSearchResult>> SuggestAsync(string input, string language)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            using (Disposable.Create(() => IsInConnecting = false))
            {
                IsInConnecting = true;

                var result = await _service.SuggestAsync(input, language);

                return result.Select(r => new GoogleSearchResult(r, _languagesService, StringTags.GoogleSuggest));
            }
        }
    }
}