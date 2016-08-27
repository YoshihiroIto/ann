﻿using System;
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
using Reactive.Bindings.Extensions;

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

        public MainWindow(App app, ConfigHolder configHolder)
        {
            Debug.Assert(app != null);
            Debug.Assert(configHolder != null);

            _app = app;
            _configHolder = configHolder;

            _DataContext = new MainWindowViewModel(_app, _configHolder);
            DataContext = _DataContext;

            SetupMessenger();

            if (double.IsNaN(_configHolder.MainWindow.Left))
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetupExecutableUnitsPanel();

            UpdateSize();

            Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                    h => StatusBar.IsVisibleChanged += h,
                    h => StatusBar.IsVisibleChanged -= h)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .ObserveOnUIDispatcher()
                .Subscribe(_ => UpdateSize())
                .AddTo(_DataContext.CompositeDisposable);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlur(this);
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
            BasePanel.Height =
                InputLineHeight +
                _DataContext.Candidates.Value.Length*ViewConstants.ExecutableUnitPanelHeight;

            var height = BasePanel.Height;

            if (_DataContext.Candidates.Value.Any())
                height += ViewConstants.BaseMarginUnit;

            if (StatusBar.Visibility == Visibility.Visible)
            {
                height += StatusBar.ActualHeight;
                Canvas.SetLeft(StatusBar, 0);
                Canvas.SetTop(StatusBar, height - StatusBar.ActualHeight - ViewConstants.MainWindowBorderThicknessUnit*2);
            }

            Height = height;
        }

        private double InputLineHeight =>
            InputLine.ActualHeight +
            InputLine.Margin.Top +
            InputLine.Margin.Bottom;

        private readonly Canvas[] _ExecutableUnitPanels = new Canvas[ViewConstants.MaxExecutableUnitPanelCount];

        private void SetupExecutableUnitsPanel()
        {
            for (var i = 0; i != ViewConstants.MaxExecutableUnitPanelCount; ++i)
            {
                _ExecutableUnitPanels[i] = Resources["ExecutableUnitPanel"] as Canvas;
                Debug.Assert(_ExecutableUnitPanels[i] != null);

                _ExecutableUnitPanels[i].DataContext = null;
                _ExecutableUnitPanels[i].PreviewMouseLeftButtonUp += ExecutableUnitPanel_PreviewMouseLeftButtonUp;
                _ExecutableUnitPanels[i].MouseLeftButtonDown += ExecutableUnitPanel_MouseLeftButtonDown;

                Canvas.SetTop(_ExecutableUnitPanels[i], InputLineHeight + i*ViewConstants.ExecutableUnitPanelHeight);
                Canvas.SetLeft(_ExecutableUnitPanels[i], ViewConstants.BaseMarginUnit);

                BasePanel.Children.Add(_ExecutableUnitPanels[i]);
            }

            _DataContext.Candidates
                .ObserveOnUIDispatcher()
                .Subscribe(candidates =>
                {
                    var index = 0;

                    foreach (var c in candidates)
                    {
                        _ExecutableUnitPanels[index].DataContext = c;
                        _ExecutableUnitPanels[index].Visibility = Visibility.Visible;

                        ++index;
                    }

                    for (var i = index; i != ViewConstants.MaxExecutableUnitPanelCount; ++i)
                    {
                        _ExecutableUnitPanels[i].DataContext = null;
                        _ExecutableUnitPanels[i].Visibility = Visibility.Collapsed;
                    }

                    UpdateSize();
                }).AddTo(_DataContext.CompositeDisposable);
        }

        private void ExecutableUnitPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ExecutableUnitViewModel;

            _DataContext.SelectedCandidate.Value = item;
        }

        private void ExecutableUnitPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ExecutableUnitViewModel;

            _DataContext.SelectedCandidate.Value = item;
            _DataContext.RunCommand.Execute(null);
        }
    }
}