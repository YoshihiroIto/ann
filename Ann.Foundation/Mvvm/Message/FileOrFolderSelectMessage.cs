using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Mvvm.Message
{
    public sealed class FileOrFolderSelectMessage
    {
        public bool IsFolderPicker {get;set;}
        public string InitialDirectory {get;set;}
        public IEnumerable<CommonFileDialogFilter> Filters { get; set; }

        //
        public string Response { get; set; }
    }
}
