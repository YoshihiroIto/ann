using System;
using System.Linq;
using System.Windows;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann
{
    public class ViewManager : DisposableModelBase
    {
        public ViewManager(LanguagesService languagesService)
        {
            SetupLanguagesService(languagesService);
        }

        public void Initialize()
        {
        }

        private readonly ResourceDictionary _CurrentResourceDictionary = new ResourceDictionary();

        private void SetupLanguagesService(LanguagesService languagesService)
        {
            languagesService.ObserveProperty(x => x.CultureName)
                .Subscribe(_ =>
                {
                    foreach (var e in Enum.GetValues(typeof(StringTags)).Cast<StringTags>())
                        _CurrentResourceDictionary[e.ToString()] = languagesService.GetString(e);
                }).AddTo(CompositeDisposable);

            Application.Current?.Resources.MergedDictionaries.Add(_CurrentResourceDictionary);
        }
    }
}