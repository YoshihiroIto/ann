using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ann.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage
{
    public class ShortcutKeyListBoxViewModel : ViewModelBase
    {
        public ObservableCollection<ShortcutKeyViewModel> Keys { get; set; } =
            new ObservableCollection<ShortcutKeyViewModel>();

        public ReactiveCommand HideKeyAddCommand { get; }

        public ShortcutKeyListBoxViewModel(IEnumerable<ShortcutKey> data)
        {
            CompositeDisposable.Add(() => Keys.ForEach(k => k.Dispose()));

            foreach (var d in data)
                Keys.Add(new ShortcutKeyViewModel(d));

            HideKeyAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            HideKeyAddCommand.Subscribe(_ =>
            {
                Keys.Add(new ShortcutKeyViewModel(new ShortcutKey()));
            }).AddTo(CompositeDisposable);
        }
    }
}