using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }

        public ReactiveCommand FolderSelectDialogOpenCommand { get; }
        
        public PathViewModel(Path model, bool isFolder, Func<IEnumerable<CommonFileDialogFilter>> makeFilters = null)
        {
            Debug.Assert(model != null);
            
            Path = model.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(CompositeDisposable);

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
        }
    }
}