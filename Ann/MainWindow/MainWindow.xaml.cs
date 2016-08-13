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
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _DataContext;

        private readonly App _app = Entry.App;

        public MainWindow()
        {
            Debug.Assert(_app != null);
            
            _DataContext = new MainWindowViewModel(_app);
            DataContext = _DataContext;

            SetupMessenger();

            if (double.IsNaN(_DataContext.Config.Left))
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
                if (WindowsHelper.IsOnTrayMouseCursor)
                    return;

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
            await FocusInputTextBlockIfVisibledAsync();
        }

        private async void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            await FocusInputTextBlockIfVisibledAsync();
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InputTextBox.Text = string.Empty;

            await WpfHelper.DoEventsAsync();
            await FocusInputTextBlockIfVisibledAsync();
        }

        private void PopupBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ExecutableUnitViewModel;

            _DataContext.SelectedCandidate.Value = item;
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

            if (_app.Config.ShortcutKeys.Activate.Key == Key.None)
            {
                _app.IsEnableActivateHotKey = true;
                return;
            }

            _activateHotKey = new HotKeyRegister(
                _app.Config.ShortcutKeys.Activate.Modifiers,
                _app.Config.ShortcutKeys.Activate.Key,
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
            InputBindings.AddRange(_app.Config.ShortcutKeys.Hide
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
        }
    }
}