using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    /// <summary>
    /// FileOrFolderListBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ShortcutkeyListBox
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
                typeof(ShortcutkeyListBox),
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
                typeof(ShortcutkeyListBox),
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
                typeof(ShortcutkeyListBox),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(IList),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion

        public ShortcutkeyListBox()
        {
            InitializeComponent();
        }
    }
}