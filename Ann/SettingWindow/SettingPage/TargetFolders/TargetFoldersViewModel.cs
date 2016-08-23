using System;
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
using Ann.Foundation.Mvvm.Message;
using GongSolutions.Wpf.DragDrop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;
using Path = Ann.Core.Path;

namespace Ann.SettingWindow.SettingPage.TargetFolders
{
    public class TargetFoldersViewModel : ViewModelBase, IDropTarget
    {
        public ReactiveProperty<bool> IsIncludeSystemFolder { get; }
        public ReactiveProperty<bool> IsIncludeSystemX86Folder { get; }
        public ReactiveProperty<bool> IsIncludeProgramsFolder { get; }
        public ReactiveProperty<bool> IsIncludeProgramFilesFolder { get; }
        public ReactiveProperty<bool> IsIncludeProgramFilesX86Folder { get; }
        public ReactiveProperty<bool> IsIncludeCommonStartMenuFolder { get; }

        public ReadOnlyReactiveCollection<PathViewModel> Folders { get; set; }

        public ReactiveCommand FolderAddCommand { get; }
        public ReactiveCommand<PathViewModel> FolderRemoveCommand { get; }

        private readonly Subject<int> _pathChanged;

        private readonly Core.Config.App _model;

        private readonly WindowMessageBroker _messenger;

        public TargetFoldersViewModel(Core.Config.App model, App app, WindowMessageBroker messenger)
        {
            Debug.Assert(model != null);
            Debug.Assert(app != null);
            Debug.Assert(messenger != null);

            _model = model;
            _messenger = messenger;

            _pathChanged = new Subject<int>().AddTo(CompositeDisposable);

            IsIncludeSystemFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeSystemFolder, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            IsIncludeSystemX86Folder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeSystemX86Folder, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramsFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramsFolder, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramFilesFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramFilesFolder, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramFilesX86Folder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramFilesX86Folder, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            IsIncludeCommonStartMenuFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeCommonStartMenu, ReactivePropertyMode.DistinctUntilChanged)
                    .AddTo(CompositeDisposable);

            Folders = model.TargetFolder.Folders.ToReadOnlyReactiveCollection(p =>
            {
                var isInitializing = true;
                using (Disposable.Create(() => isInitializing = false))
                {
                    var path = new PathViewModel(p, DialogOpeningAction);

                    path.Path
                        .Where(_ => isInitializing == false)
                        .Subscribe(_ => _pathChanged.OnNext(0))
                        .AddTo(path.CompositeDisposable);

                    // 未入力状態でフォーカスが外れたら削除する
                    path.IsFocused
                        .Where(i => i == false)
                        .Where(_ => isInitializing == false)
                        .Where(_ => string.IsNullOrEmpty(path.Path.Value))
                        .Subscribe(_ => model.TargetFolder.Folders.Remove(p))
                        .AddTo(path.CompositeDisposable);

                    return path;
                }
            }).AddTo(CompositeDisposable);

            FolderAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FolderAddCommand.Subscribe(_ => model.TargetFolder.Folders.Add(new Path(string.Empty)))
                .AddTo(CompositeDisposable);

            FolderRemoveCommand = new ReactiveCommand<PathViewModel>().AddTo(CompositeDisposable);
            FolderRemoveCommand.Subscribe(p =>
            {
                var t = Folders.FirstOrDefault(f => ReferenceEquals(p, f));
                if (t != null)
                    model.TargetFolder.Folders.Remove(t.Model);
            }).AddTo(CompositeDisposable);

            Observable
                .Merge(IsIncludeSystemFolder.ToUnit())
                .Merge(IsIncludeSystemX86Folder.ToUnit())
                .Merge(IsIncludeProgramsFolder.ToUnit())
                .Merge(IsIncludeProgramFilesFolder.ToUnit())
                .Merge(IsIncludeProgramFilesX86Folder.ToUnit())
                .Merge(IsIncludeCommonStartMenuFolder.ToUnit())
                .Merge(Folders.CollectionChangedAsObservable().ToUnit())
                .Merge(_pathChanged.ToUnit())
                .Throttle(TimeSpan.FromMilliseconds(50))
                .ObserveOn(ReactivePropertyScheduler.Default)
                .Subscribe(async _ =>
                {
                    ValidateAll();
                    await app.UpdateIndexAsync();
                })
                .AddTo(CompositeDisposable);

            Observable
                .Merge(Folders.CollectionChangedAsObservable().ToUnit())
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            ValidateAll();
        }

        private string DialogOpeningAction()
        {
            var message = new FileOrFolderSelectMessage
            {
                IsFolderPicker = true
            };

            _messenger.Publish(message);

            return message.Response;
        }

        private void ValidateAll()
        {
            foreach (var pvm in Folders)
                pvm.ValidationMessage.Value = Validate(pvm, _model.TargetFolder.Folders);
        }

        private static StringTags? Validate(PathViewModel item, IEnumerable<Path> parentCollection)
        {
            if (string.IsNullOrEmpty(item.Path.Value) == false)
                if (Directory.Exists(item.Path.Value) == false)
                    return StringTags.Message_FolderNotFound;

            if (parentCollection
                .Where(p => item.Model != p)
                .Any(p => p.Value == item.Path.Value))
                return StringTags.Message_AlreadySetSameFolder;

            return null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null)
            {
                var paths = (IEnumerable<string>)dataObject.GetData(DataFormats.FileDrop, false);
                if (paths != null && paths.Any(Directory.Exists))
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

                if (paths != null)
                    foreach (var path in paths.Where(Directory.Exists))
                        _model.TargetFolder.Folders.Add(new Path(path));
            }
            else
            {
                var vm = (PathViewModel)dropInfo.Data;
                ModelHelper.MovoTo(_model.TargetFolder.Folders, vm.Model, dropInfo.InsertIndex);
            }
        }
    }
}