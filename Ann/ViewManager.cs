using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann
{
    public class ViewManager : DisposableModelBase
    {
        private readonly Dispatcher _uiDispatcher;

        public ViewManager(Dispatcher uiDispatcher, LanguagesService languagesService)
        {
            _uiDispatcher = uiDispatcher;

            SubscribeMessages();

            SetupLanguagesService(languagesService);
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

        private void SubscribeMessages()
        {
            MessageBroker.Default
                .Subscribe<WindowActionMessage>(WindowActionAction.InvokeAction)
                .AddTo(CompositeDisposable);

            MessageBroker.Default
                .Subscribe<FileOrFolderSelectMessage>(FileOrFolderSelectAction.InvokeAction)
                .AddTo(CompositeDisposable);

            AsyncMessageBroker.Default
                .Subscribe<SettingViewModel>(
                    vm => Task.Run(() => _uiDispatcher?.Invoke(() => new SettingWindow.SettingWindow {DataContext = vm}.ShowDialog()))
                ).AddTo(CompositeDisposable);
        }
    }
}