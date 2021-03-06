﻿using System.Collections;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;

namespace Ann.SettingWindow.SettingPage
{
    /// <summary>
    /// FileOrFolderListBox.xaml の相互作用ロジック
    /// </summary>
    public partial class FileOrFolderListBox
    {
        #region AddCommand

        public ICommand AddCommand
        {
            get { return (ICommand) GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(
                nameof(AddCommand),
                typeof(ICommand),
                typeof(FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(ICommand),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion

        #region RemoveCommand

        public ICommand RemoveCommand
        {
            get { return (ICommand) GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(
                nameof(RemoveCommand),
                typeof(ICommand),
                typeof(FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(ICommand),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion

        #region Items

        public IList Items
        {
            get { return (IList) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                nameof(Items),
                typeof(IList),
                typeof(FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(IList),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion

        #region AddButtonText

        public string AddButtonText
        {
            get { return (string)GetValue(AddButtonTextProperty); }
            set { SetValue(AddButtonTextProperty, value); }
        }

        public static readonly DependencyProperty AddButtonTextProperty =
            DependencyProperty.Register(
                nameof (AddButtonText),
                typeof (string),
                typeof (FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(string),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

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
                typeof (FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(bool),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        #region DropTarget

        public IDropTarget DropTarget
        {
            get { return (IDropTarget)GetValue(DropTargetProperty); }
            set { SetValue(DropTargetProperty, value); }
        }

        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register(
                nameof (DropTarget),
                typeof (IDropTarget),
                typeof (FileOrFolderListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue            = default(IDropTarget),
                    BindsTwoWayByDefault    = true
                }
            );

        #endregion

        public FileOrFolderListBox()
        {
            InitializeComponent();
        }
    }
}