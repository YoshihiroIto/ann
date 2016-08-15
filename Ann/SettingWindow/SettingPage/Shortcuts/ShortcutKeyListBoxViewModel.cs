using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Ann.Core.Config;
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
            Keys = model.ToReadOnlyReactiveCollection(k =>
            {
                var isInitializing = true;
                using (Disposable.Create(() => isInitializing = false))
                {
                    var svm = new ShortcutKeyViewModel(k);

                    // 未入力状態でフォーカスが外れたら削除する
                    svm.IsFocused
                        .Where(i => i == false)
                        .Where(_ => isInitializing == false)
                        .Where(_ => svm.Key.Value == Key.None)
                        .Subscribe(_ => model.Remove(k))
                        .AddTo(svm.CompositeDisposable);

                    return svm;
                }
            }).AddTo(CompositeDisposable);

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