using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ann.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutKeyListBoxViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<ShortcutKeyViewModel> Keys { get; set; }

        public ReactiveCommand KeyAddCommand { get; }
        public ReactiveCommand<ShortcutKeyViewModel> KeyRemoveCommand { get; }

        public ShortcutKeyListBoxViewModel(ObservableCollection<ShortcutKey> model)
        {
            Keys = model.ToReadOnlyReactiveCollection(k => new ShortcutKeyViewModel(k))
                .AddTo(CompositeDisposable);

            KeyAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            KeyAddCommand.Subscribe(_ => model.Add(new ShortcutKey()))
                .AddTo(CompositeDisposable);

            KeyRemoveCommand = new ReactiveCommand<ShortcutKeyViewModel>().AddTo(CompositeDisposable);
            KeyRemoveCommand.Subscribe(p =>
            {
                var t = model.FirstOrDefault(f => (f.Key == p.Key.Value) &&  (f.Modifiers == p.Modifiers));
                if (t != null)
                    model.Remove(t);
            }).AddTo(CompositeDisposable);
        }
    }
}