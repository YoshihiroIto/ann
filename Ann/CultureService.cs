using System;
using System.Globalization;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class CultureService : DisposableModelBase
    {
        public static CultureService Current { get; } = new CultureService();
        public Resources Resources { get; } = new Resources();

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

        private CultureService()
        {
            App.Instance.Config.ObserveProperty(x => x.Culture)
                .Subscribe(c => CultureName = c)
                .AddTo(App.Instance.CompositeDisposable);

            App.Instance.CompositeDisposable.Add(this);
        }
    }
}