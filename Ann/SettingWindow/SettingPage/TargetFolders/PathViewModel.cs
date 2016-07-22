using System;
using System.Diagnostics;
using Ann.Config;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Livet.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.TargetFolders
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }

        public ReactiveCommand FolderSelectDialogOpenCommand { get; }
        
        public PathViewModel(InteractionMessenger messenger, bool isFolder, Path model)
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
                        IsFolderPicker = isFolder
                    });

                if (res?.Response != null)
                    Path.Value = res?.Response;
            }).AddTo(CompositeDisposable);
        }
    }
}