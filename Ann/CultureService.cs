using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class CultureService : DisposableModelBase
    {
        public static CultureService Instance { get; } = new CultureService();
        public static readonly CultureSummry[] SupportedCultures;

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

        static CultureService()
        {
            var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? string.Empty;
            var resFiles = Directory.EnumerateFiles(dir, "Ann.resources.dll", SearchOption.AllDirectories);

            SupportedCultures = resFiles.Select(f =>
            {
                var name = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f));
                if (name == null)
                    return null;

                return new CultureSummry
                {
                    Caption = CultureInfo.GetCultureInfo(name).NativeName,
                    CultureName = name
                };
            })
            .Where(x => x != null)
            .ToArray();

            if (SupportedCultures.IsEmpty())
            {
                SupportedCultures = new[]
                {
                    new CultureSummry
                    {
                        Caption = "Default",
                        CultureName = string.Empty
                    }
                };
            }
        }
    }

    public class CultureSummry
    {
        public string Caption { get; set; }
        public string CultureName { get; set; }
    }
}