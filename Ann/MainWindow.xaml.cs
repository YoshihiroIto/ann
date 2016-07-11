using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ann.Foundation;
using HotKey;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlur(this);
            SetupHotKey();
            Application.Current.Deactivated += (_, __) => Visibility = Visibility.Hidden;

            Keyboard.Focus(InputTextBox);

            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
                ExecutableUnitViewModel.Scale =
                    new Size(
                        source.CompositionTarget.TransformToDevice.M11,
                        source.CompositionTarget.TransformToDevice.M22);
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

            var item = (sender as ListBoxItem)?.DataContext as ExecutableUnitViewModel;

            vm.SelectedCandidate.Value = item;
        }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;

            vm?.RunCommand.Execute(null);
        }

        private void SetupHotKey()
        {
            var switchVisibility = new HotKeyRegister(MOD_KEY.CONTROL, System.Windows.Forms.Keys.Space, this);
            switchVisibility.HotKeyPressed += _ =>
            {
                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                    Activate();
                    Keyboard.Focus(InputTextBox);
                }
                else
                    Visibility = Visibility.Hidden;
            };
        }
    }
}