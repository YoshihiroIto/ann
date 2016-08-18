using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Properties;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Ann.Foundation;
using Path = Ann.Core.Path;

namespace Ann.SettingWindow.SettingPage.TargetFolders
{
    public class TargetFoldersViewModel : ViewModelBase
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

        public TargetFoldersViewModel(Core.Config.App model, App app)
        {
            Debug.Assert(model != null);

            _model = model;

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
                    var path = new PathViewModel(p, true);
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
                var t = model.TargetFolder.Folders.FirstOrDefault(f => f.Value == p.Path.Value);
                if (t != null)
                    model.TargetFolder.Folders.Remove(t);
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
                .Merge(CultureService.Instance.ObserveProperty(x => x.Resources).ToUnit())
                .Subscribe(_ => ValidateAll())
                .AddTo(CompositeDisposable);

            CompositeDisposable.Add(app.CancelUpdateIndex);

            ValidateAll();
        }

        private void ValidateAll()
        {
            foreach (var pvm in Folders)
                pvm.ValidationMessage.Value = Validate(pvm, _model.TargetFolder.Folders);
        }

        private string Validate(PathViewModel item, IEnumerable<Path> parentCollection)
        {
            if (string.IsNullOrEmpty(item.Path.Value) == false)
                if (Directory.Exists(item.Path.Value) == false)
                    return Resources.Message_FolderNotFound;

            if (parentCollection
                .Where(p => item.Model != p)
                .Any(p => p.Value == item.Path.Value))
                return Resources.Message_AlreadySetSameFolder;

            return null;
        }
    }
}