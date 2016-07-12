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
        private readonly MainWindowViewModel _DataContext = new MainWindowViewModel();

        public MainWindow()
        {
            DataContext = _DataContext;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlur(this);
            SetupHotKey();
            SetupIcon();
            Application.Current.Deactivated += (_, __) => Visibility = Visibility.Hidden;
            Keyboard.Focus(InputTextBox);
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

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListBoxItem)?.DataContext as ExecutableUnitViewModel;

            _DataContext.SelectedCandidate.Value = item;
            _DataContext.RunCommand.Execute(null);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const int maxLine = 8;

            _DataContext.CandidatesListMaxHeight.Value = (e.NewSize.Height + 2)*maxLine + 4;
        }

        private void SetupIcon()
        {
            var source = PresentationSource.FromVisual(this);

            if (source?.CompositionTarget == null)
                return;

            _DataContext.IconSize =
                new Size(
                    Constants.IconSize*source.CompositionTarget.TransformToDevice.M11,
                    Constants.IconSize*source.CompositionTarget.TransformToDevice.M22);
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