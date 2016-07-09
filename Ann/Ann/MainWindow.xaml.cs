using System.Windows.Controls;
using System.Windows.Input;

namespace Ann
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(InputTextBox);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var candidate = (ListBox)sender;
            if (candidate == null)
                return;

            if (candidate.SelectedItems.Count == 0)
                return;

            candidate.ScrollIntoView(candidate.SelectedItems[0]);
        }
    }
}
