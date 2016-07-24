using System;
using System.Diagnostics;
using System.Linq;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.TargetFolders
{
    public class TargetFoldersViewModel : ViewModelBase
    {
        public string Name { get; } = "Target Folders";
        public bool IsAbout { get; } = false;

        public ReactiveProperty<bool> IsIncludeSystemFolder { get; }
        public ReactiveProperty<bool> IsIncludeSystemX86Folder { get; }
        public ReactiveProperty<bool> IsIncludeProgramsFolder { get; }
        public ReactiveProperty<bool> IsIncludeProgramFilesFolder { get; }
        public ReactiveProperty<bool> IsIncludeProgramFilesX86Folder { get; }

        public ReadOnlyReactiveCollection<PathViewModel> Folders { get; set; }

        public ReactiveCommand FolderAddCommand { get; }
        public ReactiveCommand<PathViewModel> FolderRemoveCommand { get; }

        public TargetFoldersViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            IsIncludeSystemFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeSystemFolder)
                    .AddTo(CompositeDisposable);

            IsIncludeSystemX86Folder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeSystemX86Folder)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramsFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramsFolder)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramFilesFolder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramFilesFolder)
                    .AddTo(CompositeDisposable);

            IsIncludeProgramFilesX86Folder =
                model.TargetFolder.ToReactivePropertyAsSynchronized(x => x.IsIncludeProgramFilesX86Folder)
                    .AddTo(CompositeDisposable);

            Folders = model.TargetFolder.Folders.ToReadOnlyReactiveCollection(p => new PathViewModel(p, Messenger, true))
                .AddTo(CompositeDisposable);

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
        }
    }
}