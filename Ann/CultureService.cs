using System;
using System.Diagnostics;
using System.Globalization;
using Ann.Foundation;
using Ann.Foundation.Exception;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class CultureService : DisposableModelBase
    {
        private static CultureService _Instance;

        public static CultureService Instance
        {
            get
            {
                if (_Instance == null)
                    if (WpfHelper.IsDesignMode && Foundation.TestHelper.IsTestMode == false)
                        _Instance = new CultureService(new Core.Config.App());
                
                if (_Instance == null)
                    throw new UninitializedException();

                return _Instance;
            }
        }

        public static void Clean()
        {
            _Instance?.Dispose();
            _Instance = null;
        }

        public static void Initialize(Core.Config.App config)
        {
            if (_Instance != null)
                throw new NestingException();

            _Instance = new CultureService(config);
        }

        public static void Destory()
        {
            if (_Instance == null)
                throw new NestingException();

            Clean();
        }

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

        private CultureService(Core.Config.App config)
        {
            Debug.Assert(config != null);

            config.ObserveProperty(x => x.Culture)
                .Subscribe(c => CultureName = c)
                .AddTo(CompositeDisposable);
        }
    }
}