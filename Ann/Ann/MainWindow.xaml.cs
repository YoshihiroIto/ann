using System.Windows.Controls;
using System.Windows.Input;
using Ann.Core;
using Ann.Foundation;

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

            WindowHelper.EnableBlur(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var candidate = sender as ListBox;

            if (candidate?.SelectedItem == null)
                return;

            candidate.ScrollIntoView(candidate.SelectedItem);
        }

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm == null)
                return;

            var item = (sender as ListBoxItem)?.DataContext as ExecutableUnit;

            vm.SelectedCandidate.Value = item;
        }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;

            vm?.RunCommand.Execute(null);
        }
    }
}