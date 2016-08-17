using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using Path = Ann.Core.Path;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }
        public ReactiveProperty<bool> IsFocused { get; }
        public ReactiveCommand FolderSelectDialogOpenCommand { get; }

        public ReactiveProperty<string> ValidationMessage { get; }

        public Path Model { get; }

        public PathViewModel(Path model, bool isFolder, Func<IEnumerable<CommonFileDialogFilter>> makeFilters = null)
        {
            Debug.Assert(model != null);

            Model = model;

            Path = model
                .ToReactivePropertyAsSynchronized(x => x.Value)
                .AddTo(CompositeDisposable);

            IsFocused = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

            FolderSelectDialogOpenCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FolderSelectDialogOpenCommand.Subscribe(_ =>
            {
                var message = new FileOrFolderSelectMessage
                {
                    IsFolderPicker = isFolder,
                    Filters = makeFilters?.Invoke()
                };

                MessageBroker.Default.Publish(message);

                if (message.Response != null)
                    Path.Value = message.Response;
            }).AddTo(CompositeDisposable);

            ValidationMessage = new ReactiveProperty<string>().AddTo(CompositeDisposable);
        }
    }
}