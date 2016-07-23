using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ann.Foundation;
using Ann.Foundation.Control;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
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

            if (double.IsNaN(App.Instance.Config.MainWindow.Left))
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlur(this);
            InputTextBox.Focus();
            InitializeHotKey();
            InitializeShortcutKey();

            Application.Current.Deactivated += (_, __) =>
            {
                var windows = Application.Current.Windows.OfType<Window>().ToArray();
                if (windows.Length == 1 && Equals(windows[0], this))
                    Visibility = Visibility.Hidden;
            };
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

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _DataContext.CandidateItemHeight.Value = e.NewSize.Height;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ExecutableUnitViewModel;

            _DataContext.SelectedCandidate.Value = item;
            _DataContext.RunCommand.Execute(null);
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            await FocusInputTextBlockIfVisibled();
        }

        private async void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            await FocusInputTextBlockIfVisibled();
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await FocusInputTextBlockIfVisibled();
        }

        private async Task FocusInputTextBlockIfVisibled()
        {
            if (Visibility != Visibility.Visible)
                return;

            await Task.Delay(TimeSpan.FromMilliseconds(20));
            InputTextBox.Focus();
        }

        private void InitializeHotKey()
        {
            SetupHotKey();

            _DataContext.CompositeDisposable.Add(() => _activateHotKey?.Dispose());
            
            Observable.FromEventPattern(
                h => App.Instance.ShortcutKeyChanged += h,
                h => App.Instance.ShortcutKeyChanged -= h)
                .Subscribe(_ => SetupHotKey())
                .AddTo(_DataContext.CompositeDisposable);
        }

        private void InitializeShortcutKey()
        {
            SetupShortcutKey();
            
            Observable.FromEventPattern(
                h => App.Instance.ShortcutKeyChanged += h,
                h => App.Instance.ShortcutKeyChanged -= h)
                .Subscribe(_ => SetupShortcutKey())
                .AddTo(_DataContext.CompositeDisposable);
        }

        private HotKeyRegister _activateHotKey;

        private void SetupHotKey()
        {
            _activateHotKey?.Dispose();
            _activateHotKey = null;

            if (App.Instance.Config.MainWindow.ShortcutKeys.Activate.Key == Key.None)
            {
                _DataContext.IsEnableActivateHotKey.Value = true;
                return;
            }

            _activateHotKey = new HotKeyRegister(
                App.Instance.Config.MainWindow.ShortcutKeys.Activate.Modifiers,
                App.Instance.Config.MainWindow.ShortcutKeys.Activate.Key,
                Application.Current.MainWindow);

            _activateHotKey.HotKeyPressed += (_, __) =>
            {
                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                    Activate();
                }
                else
                    Visibility = Visibility.Hidden;
            };

            _DataContext.IsEnableActivateHotKey.Value = _activateHotKey.Register();

            if (_DataContext.IsEnableActivateHotKey.Value == false)
            {
                _activateHotKey.Dispose();
                _activateHotKey = null;
            }
        }

        private void SetupShortcutKey()
        {
            InputBindings.Clear();

            // 標準
            InputBindings.Add(new KeyBinding {Key = Key.Enter, Command = _DataContext.RunCommand});
            InputBindings.Add(new KeyBinding {Key = Key.Escape, Command = _DataContext.AppHideCommand});

            // コンフィグから指定されたもの
            InputBindings.AddRange(App.Instance.Config.MainWindow.ShortcutKeys.Hide
                .Select(k =>
                    new KeyBinding
                    {
                        Key = k.Key,
                        Modifiers = k.Modifiers,
                        Command = _DataContext.AppHideCommand
                    }).ToArray());
        }
    }
}