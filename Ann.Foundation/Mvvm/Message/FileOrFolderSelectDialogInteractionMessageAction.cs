using System.Windows;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Mvvm.Message
{
    public class FileOrFolderSelectDialogInteractionMessageAction : InteractionMessageAction<FrameworkElement>
    {
        protected override void InvokeAction(InteractionMessage m)
        {
            var folderSelectionMessage = m as FileOrFolderSelectMessage;

            if (folderSelectionMessage == null)
                return;

            using (var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = folderSelectionMessage.IsFolderPicker,
                InitialDirectory = folderSelectionMessage.InitialDirectory
            })
            {
                if (folderSelectionMessage.Filters != null)
                {
                    foreach (var filter in folderSelectionMessage.Filters)
                        dialog.Filters.Add(filter);
                }

                folderSelectionMessage.Response =
                    dialog.ShowDialog() == CommonFileDialogResult.Ok
                        ? dialog.FileName
                        : null;
            }
        }
    }
}