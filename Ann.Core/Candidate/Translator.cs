using System;
using System.Threading.Tasks;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Core.Candidate
{
    public class Translator : DisposableModelBase
    {
        private readonly LanguagesService _languagesService;
        private readonly TranslateService _service;

        #region IsInAuthentication

        private bool _IsInAuthentication;

        public bool IsInAuthentication
        {
            get { return _IsInAuthentication; }
            private set { SetProperty(ref _IsInAuthentication, value); }
        }

        #endregion

        public Translator(string clientId, string clientSecret, LanguagesService languagesService)
        {
            _languagesService = languagesService;
            _service = new TranslateService(clientId, clientSecret);

            _service.ObserveProperty(i => i.IsInAuthentication)
                .Subscribe(i => IsInAuthentication = i)
                .AddTo(CompositeDisposable);
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