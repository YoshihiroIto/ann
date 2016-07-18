using System;
using System.Diagnostics;
using System.Windows.Input;
using Ann.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow
{
    public class ShortcutKeyViewModel : ViewModelBase
    {
        public ReactiveProperty<Key> Key { get; }
        public ReactiveProperty<bool> IsControl { get; }
        public ReactiveProperty<bool> IsAlt { get; }
        public ReactiveProperty<bool> IsShift { get; }

        public ShortcutKeyViewModel(ShortcutKey data)
        {
            Debug.Assert(data != null);

            Key = new ReactiveProperty<Key>(data.Key).AddTo(CompositeDisposable);

            IsControl = new ReactiveProperty<bool>((data.Modifiers & ModifierKeys.Control) != 0)
                .AddTo(CompositeDisposable);
            IsAlt = new ReactiveProperty<bool>((data.Modifiers & ModifierKeys.Alt) != 0)
                .AddTo(CompositeDisposable);
            IsShift = new ReactiveProperty<bool>((data.Modifiers & ModifierKeys.Shift) != 0)
                .AddTo(CompositeDisposable);

            Key.Subscribe(k => data.Key = k).AddTo(CompositeDisposable);

            IsControl.Subscribe(i =>
            {
                if (i)
                    data.Modifiers |= ModifierKeys.Control;
                else
                    data.Modifiers &= ~ModifierKeys.Control;
            }).AddTo(CompositeDisposable);

            IsAlt.Subscribe(i =>
            {
                if (i)
                    data.Modifiers |= ModifierKeys.Alt;
                else
                    data.Modifiers &= ~ModifierKeys.Alt;
            }).AddTo(CompositeDisposable);

            IsShift.Subscribe(i =>
            {
                if (i)
                    data.Modifiers |= ModifierKeys.Shift;
                else
                    data.Modifiers &= ~ModifierKeys.Shift;
            }).AddTo(CompositeDisposable);
        }
    }
}