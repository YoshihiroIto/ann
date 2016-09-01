using System.Reactive.Disposables;
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

        #region IsInConnecting

        private bool _IsInConnecting;

        public bool IsInConnecting
        {
            get { return _IsInConnecting; }
            set { SetProperty(ref _IsInConnecting, value); }
        }

        #endregion

        public Translator(string clientId, string clientSecret, LanguagesService languagesService)
        {
            _languagesService = languagesService;
            _service = new TranslateService(clientId, clientSecret).AddTo(CompositeDisposable);
        }

        public void CancelTranslate()
        {
            _service.CancelTranslate();
        }

        public async Task<TranslateResult> TranslateAsync(string input, TranslateService.LanguageCodes from, TranslateService.LanguageCodes to)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (_service.IsExpierd)
            {
                using (Disposable.Create(() => IsInAuthentication = false))
                {
                    IsInAuthentication = true;

                    var r = await _service.AuthenticateAsync();
                    if (r == false)
                        return null;
                }
            }

            using (Disposable.Create(() => IsInConnecting = false))
            {
                IsInConnecting = true;

                var r = await _service.TranslateAsync(input, from, to);
                if (r == null)
                    return null;

                return new TranslateResult(r, _languagesService);
            }
        }
    }
}