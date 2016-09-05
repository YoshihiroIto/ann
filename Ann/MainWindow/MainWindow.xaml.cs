using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SimpleInjector;

namespace Ann.MainWindow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _DataContext;
        private readonly App _app;
        private readonly ConfigHolder _configHolder;

        public MainWindow(Container diContainer, App app, ConfigHolder configHolder)
        {
            Debug.Assert(app != null);
            Debug.Assert(configHolder != null);

            _app = app;
            _configHolder = configHolder;

            _DataContext = diContainer.GetInstance<MainWindowViewModel>();
            DataContext = _DataContext;

            SetupMessenger();

            if (double.IsNaN(_configHolder.MainWindow.Left))
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetupCandidatePanels();

            UpdateSize();

            var statusBarIsVisibleChanged =
                Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                    h => StatusBar.IsVisibleChanged += h,
                    h => StatusBar.IsVisibleChanged -= h);

            var statusBarSizeChanged =
                Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(
                    h => StatusBar.SizeChanged += h,
                    h => StatusBar.SizeChanged -= h);

            var sizeChanged =
                Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(
                    h => SizeChanged += h,
                    h => SizeChanged -= h);

            Observable
                .Merge(statusBarIsVisibleChanged.ToUnit())
                .Merge(statusBarSizeChanged.ToUnit())
                .Merge(sizeChanged.ToUnit())
                //.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(ReactivePropertyScheduler.Default)
                .Subscribe(_ => UpdateSize())
                .AddTo(_DataContext.CompositeDisposable);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
            InitializeHotKey();
            InitializeShortcutKey();

            Application.Current.Deactivated += (_, __) =>
            {
                if (WindowsHelper.IsOnTrayMouseCursor)
                    return;

                var windows = Application.Current.Windows.OfType<Window>().ToArray();
                if (windows.Length == 1 && Equals(windows[0], this))
                    _DataContext.Messenger.Publish(new WindowActionMessage(WindowAction.Hidden));
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            await FocusInputTextBlockIfVisibledAsync();
        }

        private async void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            await FocusInputTextBlockIfVisibledAsync();
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateSize();

            InputTextBox.Text = string.Empty;

            WpfHelper.DoEvents();
            await FocusInputTextBlockIfVisibledAsync();
        }

        private async Task FocusInputTextBlockIfVisibledAsync()
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
                    h => _app.ShortcutKeyChanged += h,
                    h => _app.ShortcutKeyChanged -= h)
                .Subscribe(_ => SetupHotKey())
                .AddTo(_DataContext.CompositeDisposable);
        }

        private void InitializeShortcutKey()
        {
            SetupShortcutKey();

            Observable.FromEventPattern(
                    h => _app.ShortcutKeyChanged += h,
                    h => _app.ShortcutKeyChanged -= h)
                .Subscribe(_ => SetupShortcutKey())
                .AddTo(_DataContext.CompositeDisposable);
        }

        private HotKeyRegister _activateHotKey;

        private void SetupHotKey()
        {
            _activateHotKey?.Dispose();
            _activateHotKey = null;

            if (_configHolder.Config.ShortcutKeys.Activate.Key == Key.None)
            {
                _app.IsEnableActivateHotKey = true;
                return;
            }

            _activateHotKey = new HotKeyRegister(
                _configHolder.Config.ShortcutKeys.Activate.Modifiers,
                _configHolder.Config.ShortcutKeys.Activate.Key,
                Application.Current.MainWindow);

            _activateHotKey.HotKeyPressed +=
                (_, __) =>
                    _DataContext.Messenger.Publish(
                        new WindowActionMessage(Visibility == Visibility.Hidden
                            ? WindowAction.VisibleActive
                            : WindowAction.Hidden));

            _app.IsEnableActivateHotKey = _activateHotKey.Register();

            if (_app.IsEnableActivateHotKey == false)
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
            InputBindings.Add(new KeyBinding {Key = Key.Escape, Command = _DataContext.HideCommand});

            // コンフィグから指定されたもの
            InputBindings.AddRange(_configHolder.Config.ShortcutKeys.Hide
                .Select(k =>
                    new KeyBinding
                    {
                        Key = k.Key,
                        Modifiers = k.Modifiers,
                        Command = _DataContext.HideCommand
                    }).ToArray());
        }

        private void SetupMessenger()
        {
            _DataContext.Messenger.Window = this;

            _DataContext.Messenger
                .Subscribe<WindowActionMessage>(WindowActionAction.InvokeAction)
                .AddTo(_DataContext.CompositeDisposable);

            _DataContext.Messenger
                .Subscribe<MessengerMessage>(m =>
                {
                    switch (m)
                    {
                        case MessengerMessage.InputTextBoxSetCaretLast:
                            _isInputTextBoxSetCaretLast = true;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(m), m, null);
                    }
                })
                .AddTo(_DataContext.CompositeDisposable);

            _DataContext.AsyncMessenger
                .Subscribe<SettingViewModel>(
                    vm =>
                        Task.Run(
                            () =>
                                Dispatcher.Invoke(
                                    () => new SettingWindow.SettingWindow {DataContext = vm}.ShowDialog()))
                ).AddTo(_DataContext.CompositeDisposable);
        }

        private void UpdateSize()
        {
            if (_DataContext.Candidates.Value == null)
                return;

            var height = InputLineHeight;
            height += _DataContext.Candidates.Value.Length *ViewConstants.CandidatePanelHeight;

            if (_DataContext.Candidates.Value.Any())
                height += ViewConstants.BaseMarginUnit;

            ShadowLeft.Height = height - ViewConstants.MainWindowCournerCornerRadiusUnit*2;
            ShadowRight.Height = height - ViewConstants.MainWindowCournerCornerRadiusUnit*2;
            ShadowPanel.Height = height;
            MainPanel.Height = height;

            Canvas.SetTop(StatusBar, height);
            Height = height + ViewConstants.ShadowSize*2 + StatusBar.ActualHeight;
        }

        private double InputLineHeight =>
            InputLine.ActualHeight +
            InputLine.Margin.Top +
            InputLine.Margin.Bottom;

        private readonly Canvas[] _CandidatePanels = new Canvas[ViewConstants.MaxCandidateCount];

        private void SetupCandidatePanels()
        {
            for (var i = 0; i != ViewConstants.MaxCandidateCount; ++i)
            {
                _CandidatePanels[i] = Resources["CandidatePanel"] as Canvas;
                Debug.Assert(_CandidatePanels[i] != null);

                _CandidatePanels[i].DataContext = null;
                _CandidatePanels[i].PreviewMouseLeftButtonUp += CandidatePanel_PreviewMouseLeftButtonUp;
                _CandidatePanels[i].MouseLeftButtonDown += CandidatePanel_MouseLeftButtonDown;

                Canvas.SetLeft(_CandidatePanels[i], ViewConstants.BaseMarginUnit);
                Canvas.SetTop(_CandidatePanels[i], InputLineHeight + i*ViewConstants.CandidatePanelHeight);

                MainPanel.Children.Add(_CandidatePanels[i]);
            }

            _DataContext.Candidates
                .ObserveOn(ReactivePropertyScheduler.Default)
                .Subscribe(candidates =>
                {
                    var index = 0;

                    foreach (var c in candidates)
                    {
                        _CandidatePanels[index].DataContext = c;
                        _CandidatePanels[index].Visibility = Visibility.Visible;

                        ++index;
                    }

                    for (var i = index; i != ViewConstants.MaxCandidateCount; ++i)
                    {
                        _CandidatePanels[i].DataContext = null;
                        _CandidatePanels[i].Visibility = Visibility.Collapsed;
                    }

                    UpdateSize();

                    _DataContext.DisposeOldCandidates();
                }).AddTo(_DataContext.CompositeDisposable);
        }

        private void CandidatePanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as CandidatePanelViewModel;

            _DataContext.SelectedCandidate.Value = item;
        }

        private void CandidatePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as CandidatePanelViewModel;

            _DataContext.SelectedCandidate.Value = item;
            _DataContext.RunCommand.Execute(null);
        }

        private bool _isInputTextBoxSetCaretLast;

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInputTextBoxSetCaretLast == false)
                return;

            InputTextBox.CaretIndex = InputTextBox.Text.Length;
            _isInputTextBoxSetCaretLast = false;
        }
    }
}