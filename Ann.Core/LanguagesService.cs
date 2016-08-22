using System;
using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Core
{
    public class LanguagesService : DisposableModelBase
    {
        #region CultureName

        private string _CultureName = string.Empty;

        public string CultureName
        {
            get { return _CultureName; }
            set
            {
                if (Enum.TryParse(value, false, out _CurrentLanguage) == false)
                    _CurrentLanguage = Languages.en;

                SetProperty(ref _CultureName, value);
            }
        }

        #endregion

        private Languages _CurrentLanguage;

        public LanguagesService(Config.App config)
        {
            Debug.Assert(config != null);

            config.ObserveProperty(x => x.Culture)
                .Subscribe(c => CultureName = c)
                .AddTo(CompositeDisposable);
        }

        public string GetString(StringTags tag) => Localization.GetString(_CurrentLanguage, tag);
    }
}