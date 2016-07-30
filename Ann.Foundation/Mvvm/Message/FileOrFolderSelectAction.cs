using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Mvvm.Message
{
    public static class FileOrFolderSelectAction
    {
        public static void InvokeAction(FileOrFolderSelectMessage message)
        {
            using (var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = message.IsFolderPicker,
                InitialDirectory = message.InitialDirectory
            })
            {
                if (message.Filters != null)
                {
                    foreach (var filter in message.Filters)
                        dialog.Filters.Add(filter);
                }

                var window = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive);

                message.Response =
                    dialog.ShowDialog(window) == CommonFileDialogResult.Ok
                        ? dialog.FileName
                        : null;
            }
        }
    }
}