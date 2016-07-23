using System.Collections.Generic;
using System.Windows;
using Livet.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Mvvm.Message
{
    public sealed class FileOrFolderSelectMessage : ResponsiveInteractionMessage<string>
    {
        #region IsFolderPicker

        public bool IsFolderPicker
        {
            get { return (bool)GetValue(IsFolderPickerProperty); }
            set { SetValue(IsFolderPickerProperty, value); }
        }

        public static readonly DependencyProperty IsFolderPickerProperty =
            DependencyProperty.Register(
                nameof (IsFolderPicker),
                typeof (bool),
                typeof (FileOrFolderSelectMessage),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(bool),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        #region InitialDirectory

        public string InitialDirectory
        {
            get { return (string)GetValue(InitialDirectoryProperty); }
            set { SetValue(InitialDirectoryProperty, value); }
        }

        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register(
                nameof (InitialDirectory),
                typeof (string),
                typeof (FileOrFolderSelectMessage),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(string),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        #region Filters

        public IEnumerable<CommonFileDialogFilter> Filters
        {
            get { return (IEnumerable<CommonFileDialogFilter>)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register(
                nameof (Filters),
                typeof (IEnumerable<CommonFileDialogFilter>),
                typeof (FileOrFolderSelectMessage),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(IEnumerable<CommonFileDialogFilter>),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        public FileOrFolderSelectMessage()
        {
        }

        public FileOrFolderSelectMessage(string messageKey)
            : base(messageKey)
        {
        }

        protected override Freezable CreateInstanceCore()
        {
            return new FileOrFolderSelectMessage(MessageKey);
        }
    }
}
