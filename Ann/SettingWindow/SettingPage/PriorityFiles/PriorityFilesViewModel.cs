﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using GongSolutions.Wpf.DragDrop;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;
using Path = Ann.Core.Path;

namespace Ann.SettingWindow.SettingPage.PriorityFiles
{
    public class PriorityFilesViewModel : ViewModelBase, IDropTarget
    {
        public ReadOnlyReactiveCollection<PathViewModel> Files { get; set; }

        public ReactiveCommand FileAddCommand { get; }
        public ReactiveCommand<PathViewModel> FileRemoveCommand { get; }

        private readonly Subject<int> _pathChanged;

        private readonly Core.Config.App _model;

        public PriorityFilesViewModel(Core.Config.App model, App app)
        {
            Debug.Assert(model != null);

            _model = model;

            _pathChanged = new Subject<int>().AddTo(CompositeDisposable);

            Files = model.PriorityFiles.ToReadOnlyReactiveCollection(p =>
            {
                var isInitializing = true;
                using (Disposable.Create(() => isInitializing = false))
                {
                    var pvm = new PathViewModel(p, false,
                        () => new[]
                        {
                            new CommonFileDialogFilter(Resources.ExecutableFile,
                                string.Join(", ", model.ExecutableFileExts.Select(e => "*." + e))),
                            new CommonFileDialogFilter(Resources.AllFiles, "*.*")
                        });

                    pvm.Path
                        .Where(_ => isInitializing == false)
                        .Subscribe(_ => _pathChanged.OnNext(0))
                        .AddTo(pvm.CompositeDisposable);

                    // 未入力状態でフォーカスが外れたら削除する
                    pvm.IsFocused
                        .Where(i => i == false)
                        .Where(_ => isInitializing == false)
                        .Where(_ => string.IsNullOrEmpty(pvm.Path.Value))
                        .Subscribe(_ => model.PriorityFiles.Remove(p))
                        .AddTo(pvm.CompositeDisposable);

                    return pvm;
                }
            }).AddTo(CompositeDisposable);

            FileAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FileAddCommand.Subscribe(_ => model.PriorityFiles.Add(new Path(string.Empty)))
                .AddTo(CompositeDisposable);

            FileRemoveCommand = new ReactiveCommand<PathViewModel>().AddTo(CompositeDisposable);
            FileRemoveCommand.Subscribe(p =>
            {
                var t = Files.FirstOrDefault(f => ReferenceEquals(p, f));
                if (t != null)
                    model.PriorityFiles.Remove(t.Model);
            }).AddTo(CompositeDisposable);

            _pathChanged
                .Subscribe(_ =>
                {
                    ValidateAll();
                    app.RefreshPriorityFiles();
                    app.InvokePriorityFilesChanged();
                }).AddTo(CompositeDisposable);

            Observable
                .Merge(Files.CollectionChangedAsObservable().ToUnit())
                .Merge(CultureService.Instance.ObserveProperty(x => x.Resources).ToUnit())
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            ValidateAll();
        }

        private void ValidateAll()
        {
            foreach (var pvm in Files)
                pvm.ValidationMessage.Value = Validate(pvm, _model.PriorityFiles);
        }

        private string Validate(PathViewModel item, IEnumerable<Path> parentCollection)
        {
            if (string.IsNullOrEmpty(item.Path.Value) == false)
                if (File.Exists(item.Path.Value) == false)
                    return Resources.Message_FileNotFound;

            if (parentCollection
                .Where(p => item.Model != p)
                .Any(p => p.Value == item.Path.Value))
                return Resources.Message_AlreadySetSameFile;

            return null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null)
            {
                var paths = (IEnumerable<string>)dataObject.GetData(DataFormats.FileDrop, false);
                if (paths != null && paths.Any(x => File.Exists(x) && IsEnableExt(x)))
                    dropInfo.Effects = DragDropEffects.Move;
            }
            else
                DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null)
            {
                var paths = (IEnumerable<string>)dataObject.GetData(DataFormats.FileDrop, false);

                foreach (var path in paths.Where(x => File.Exists(x) && IsEnableExt(x)))
                   _model.PriorityFiles.Add(new Path(path));
            }
            else
            {
                var vm = (PathViewModel)dropInfo.Data;
                ModelHelper.MovoTo(_model.PriorityFiles, vm.Model, dropInfo.InsertIndex);
            }
        }

        private bool IsEnableExt(string filePath)
        {
            var ext = System.IO.Path.GetExtension(filePath)?.ToLower();

            return _model.ExecutableFileExts.Any(e => ext == "." + e);
        }
    }
}