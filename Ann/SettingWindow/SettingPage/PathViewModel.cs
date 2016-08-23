using System;
using System.Diagnostics;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Path = Ann.Core.Path;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }
        public ReactiveProperty<bool> IsFocused { get; }
        public ReactiveCommand FolderSelectDialogOpenCommand { get; }

        public ReactiveProperty<StringTags?> ValidationMessage { get; }

        public Path Model { get; }

        public PathViewModel(Path model, Func<string> dialogOpeningAction)
        {
            Debug.Assert(model != null);
            Debug.Assert(dialogOpeningAction != null);

            Model = model;

            Path = model
                .ToReactivePropertyAsSynchronized(x => x.Value)
                .AddTo(CompositeDisposable);

            IsFocused = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

            FolderSelectDialogOpenCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FolderSelectDialogOpenCommand.Subscribe(_ =>
            {
                var result = dialogOpeningAction();
                if (result != null)
                    Path.Value = result;
            }).AddTo(CompositeDisposable);

            ValidationMessage = new ReactiveProperty<StringTags?>().AddTo(CompositeDisposable);
        }
    }
}