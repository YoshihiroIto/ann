using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class CultureService : NotificationObject
    {
        public static CultureService Instance { get; } = new CultureService();

        public Resources Resources { get; } = new Resources();

        private IDisposable _configObserve;

        #region CultureName

        private string _CultureName = string.Empty;

        public string CultureName
        {
            get { return _CultureName; }
            set
            {
                if (SetProperty(ref _CultureName, value) == false)
                    return;

                Resources.Culture = CultureInfo.GetCultureInfo(value);

                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(Resources));
            }
        }

        #endregion

        public void SetConfig(Core.Config.App config)
        {
            Debug.Assert(config != null);

            _configObserve?.Dispose();
            _configObserve = config.ObserveProperty(x => x.Culture)
                .Subscribe(c => CultureName = c);
        }

        public void Destory()
        {
            _configObserve?.Dispose();
            _configObserve = null;
        }

        private CultureService()
        {
            var name = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            CultureName = Constants.SupportedCultures.Any(x => x.CultureName == name) ? name : "en";
        }
    }
}