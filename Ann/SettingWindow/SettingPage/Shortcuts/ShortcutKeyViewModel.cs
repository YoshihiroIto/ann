using System;
using System.Diagnostics;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutKeyViewModel : ViewModelBase
    {
        public ReactiveProperty<Key> Key { get; }
        public ReactiveProperty<bool> IsControl { get; }
        public ReactiveProperty<bool> IsAlt { get; }
        public ReactiveProperty<bool> IsShift { get; }

        public ReactiveProperty<bool> IsFocused  { get; }

        public ModifierKeys Modifiers
        {
            get
            {
                var m = ModifierKeys.None;

                if (IsControl.Value)
                    m |= ModifierKeys.Control;
                if (IsAlt.Value)
                    m |= ModifierKeys.Alt;
                if (IsShift.Value)
                    m |= ModifierKeys.Shift;

                return m;
            }
        }

        public ShortcutKeyViewModel(ShortcutKey model)
        {
            Debug.Assert(model != null);

            Key = model.ToReactivePropertyAsSynchronized(x => x.Key).AddTo(CompositeDisposable);

            IsControl = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            IsAlt = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            IsShift = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

            IsFocused = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

            model.ObserveProperty(x => x.Modifiers).Subscribe(m =>
            {
                IsControl.Value = (model.Modifiers & ModifierKeys.Control) != 0;
                IsAlt.Value = (model.Modifiers & ModifierKeys.Alt) != 0;
                IsShift.Value = (model.Modifiers & ModifierKeys.Shift) != 0;
            }).AddTo(CompositeDisposable);

            IsControl.Subscribe(i =>
            {
                if (i)
                    model.Modifiers |= ModifierKeys.Control;
                else
                    model.Modifiers &= ~ModifierKeys.Control;
            }).AddTo(CompositeDisposable);

            IsAlt.Subscribe(i =>
            {
                if (i)
                    model.Modifiers |= ModifierKeys.Alt;
                else
                    model.Modifiers &= ~ModifierKeys.Alt;
            }).AddTo(CompositeDisposable);

            IsShift.Subscribe(i =>
            {
                if (i)
                    model.Modifiers |= ModifierKeys.Shift;
                else
                    model.Modifiers &= ~ModifierKeys.Shift;
            }).AddTo(CompositeDisposable);
        }
    }
}