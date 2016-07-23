using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Livet.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }

        public ReactiveCommand FolderSelectDialogOpenCommand { get; }
        
        public PathViewModel(Path model, InteractionMessenger messenger, bool isFolder, IEnumerable<CommonFileDialogFilter> filters = null)
        {
            Debug.Assert(messenger != null);
            Debug.Assert(model != null);
            
            Path = model.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(CompositeDisposable);

            FolderSelectDialogOpenCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FolderSelectDialogOpenCommand.Subscribe(_ =>
            {
                var res = messenger.GetResponse(
                    new FileOrFolderSelectMessage("FileOrFolderSelect")
                    {
                        IsFolderPicker = isFolder,
                        Filters = filters
                    });

                if (res?.Response != null)
                    Path.Value = res.Response;
            }).AddTo(CompositeDisposable);
        }
    }
}