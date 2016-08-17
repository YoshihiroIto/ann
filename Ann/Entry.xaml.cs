using System;
using System.IO;
using System.Runtime;
using System.Windows;
using Ann.Core;
using Ann.Foundation;

namespace Ann
{
    /// <summary>
    /// Entry.xaml の相互作用ロジック
    /// </summary>
    public partial class Entry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Constants.ConfigDirPath);
            ProfileOptimization.SetProfileRoot(Constants.ConfigDirPath);
            ProfileOptimization.StartProfile("Startup.Profile");

            DisposableChecker.Start(m => MessageBox.Show(m));
            {
                var e = new Entry();
                e.InitializeComponent();
                e.Run();
            }
            DisposableChecker.End();

            if (_isRestartRequested)
                VersionUpdater.Restart();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _configHolder = new ConfigHolder(Constants.ConfigDirPath);
            _app = new App(_configHolder);
            _viewManager = new ViewManager(Dispatcher);

            CultureService.Instance.SetConfig(_configHolder.Config);
            Reactive.Bindings.UIDispatcherScheduler.Initialize();

            MainWindow = new MainWindow.MainWindow(_app, _configHolder);
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _isRestartRequested = _app.VersionUpdater.IsRestartRequested;

            _app.Dispose();
            _viewManager.Dispose();

            CultureService.Instance.Destory();
        }

        private ConfigHolder _configHolder;
        private App _app;
        private ViewManager _viewManager;

        private static bool _isRestartRequested;
    }
}