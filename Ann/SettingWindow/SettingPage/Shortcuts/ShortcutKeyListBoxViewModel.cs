using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using GongSolutions.Wpf.DragDrop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutKeyListBoxViewModel : ViewModelBase, IDropTarget
    {
        public ReadOnlyReactiveCollection<ShortcutKeyViewModel> Keys { get; set; }

        public ReactiveCommand KeyAddCommand { get; }
        public ReactiveCommand<ShortcutKeyViewModel> KeyRemoveCommand { get; }

        private readonly Subject<int> _keyStrokeChanged;

        private readonly ObservableCollection<ShortcutKey> _model;

        public ShortcutKeyListBoxViewModel(ObservableCollection<ShortcutKey> model)
        {
            Debug.Assert(model != null);

            _model = model;

            _keyStrokeChanged = new Subject<int>().AddTo(CompositeDisposable);

            Keys = model.ToReadOnlyReactiveCollection(k =>
            {
                var isInitializing = true;
                using (Disposable.Create(() => isInitializing = false))
                {
                    var svm = new ShortcutKeyViewModel(k);

                    svm.Text
                        .Where(_ => isInitializing == false)
                        .Subscribe(_ => _keyStrokeChanged.OnNext(0))
                        .AddTo(svm.CompositeDisposable);

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
                var t = Keys.FirstOrDefault(k => ReferenceEquals(p, k));
                if (t != null)
                    model.Remove(t.Model);
            }).AddTo(CompositeDisposable);

            _keyStrokeChanged
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            Observable
                .Merge(Keys.CollectionChangedAsObservable().ToUnit())
                .Merge(CultureService.Instance.ObserveProperty(x => x.Resources).ToUnit())
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            ValidateAll();
        }

        private void ValidateAll()
        {
            foreach (var pvm in Keys)
                pvm.ValidationMessage.Value = Validate(pvm, _model);
        }

        private string Validate(ShortcutKeyViewModel item, IEnumerable<ShortcutKey> parentCollection)
        {
            if (parentCollection
                .Where(p => item.Model != p)
                .Any(p => p.Text == item.Text.Value))
                return Resources.Message_AlreadySetSameKeyStroke;

            return null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            var vm = dropInfo.Data as ShortcutKeyViewModel;
            if (vm == null)
                return;

            var oldIndex = _model.IndexOf(vm.Model);
            Debug.Assert(oldIndex != -1);

            var newIndex = Math.Min(dropInfo.InsertIndex, _model.Count);

            if (oldIndex < newIndex)
                -- newIndex;

            _model.Move(oldIndex, newIndex);
        }
    }
}