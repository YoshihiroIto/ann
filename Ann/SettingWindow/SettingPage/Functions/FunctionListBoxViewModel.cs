using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Ann.Core;
using Ann.Foundation.Mvvm;
using GongSolutions.Wpf.DragDrop;
using Reactive.Bindings;
using Ann.Core.Config;
using Reactive.Bindings.Extensions;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Ann.SettingWindow.SettingPage.Functions
{
    public class FunctionListBoxViewModel : ViewModelBase, IDropTarget
    {
        public ReadOnlyReactiveCollection<FunctionViewModel> Functions { get; set; }

        public ReactiveCommand FunctionAddCommand { get; }
        public ReactiveCommand<FunctionViewModel> FunctionRemoveCommand { get; }

        private readonly ObservableCollection<Function> _model;

        public FunctionListBoxViewModel(ObservableCollection<Function> model)
        {
            Debug.Assert(model != null);

            _model = model;

            Functions = model.ToReadOnlyReactiveCollection(f => new FunctionViewModel(f))
                .AddTo(CompositeDisposable);

            FunctionAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FunctionAddCommand.Subscribe(_ => model.Add(new Function()))
                .AddTo(CompositeDisposable);

            FunctionRemoveCommand = new ReactiveCommand<FunctionViewModel>().AddTo(CompositeDisposable);
            FunctionRemoveCommand.Subscribe(p =>
            {
                var t = Functions.FirstOrDefault(f => ReferenceEquals(p, f));
                if (t != null)
                    model.Remove(t.Model);
            }).AddTo(CompositeDisposable);

            Observable
                .Merge(Functions.CollectionChangedAsObservable().ToUnit())
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            ValidateAll();
        }

        private void ValidateAll()
        {
            foreach (var fvm in Functions)
                fvm.ValidationMessage.Value = Validate(fvm, _model);
        }

        private static StringTags? Validate(FunctionViewModel item, IEnumerable<Function> parentCollection)
        {
            // todo:実装する

            return null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            var vm = (FunctionViewModel)dropInfo.Data;

            ModelHelper.MovoTo(_model, vm.Model, dropInfo.InsertIndex);
        }
    }
}