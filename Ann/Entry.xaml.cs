using System;
using System.IO;
using System.Runtime;
using System.Windows;
using Ann.Core;
using Ann.Foundation;
using SimpleInjector;
using SimpleInjector.Diagnostics;

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

            SetupDiContainer();

            _DiContainer.GetInstance<ViewManager>().Initialize();

            Reactive.Bindings.UIDispatcherScheduler.Initialize();

            MainWindow = _DiContainer.GetInstance<MainWindow.MainWindow>();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _isRestartRequested = _DiContainer.GetInstance<App>().VersionUpdater.IsRestartRequested;

            _DiContainer.Dispose();
        }

        private void SetupDiContainer()
        {
            _DiContainer.RegisterSingleton(() => new ConfigHolder(Constants.ConfigDirPath));
            _DiContainer.RegisterSingleton(() => _DiContainer.GetInstance<ConfigHolder>().Config);
            _DiContainer.RegisterSingleton<App>();
            _DiContainer.RegisterSingleton<LanguagesService>();
            _DiContainer.RegisterSingleton<ViewManager>();
            _DiContainer.Register(() => _DiContainer.GetInstance<App>().VersionUpdater);

            _DiContainer.GetRegistration(typeof(Container)).Registration
                .SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "suppress");

            _DiContainer.GetRegistration(typeof(VersionUpdater)).Registration
                .SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "suppress");

#if DEBUG
            _DiContainer.Verify();
#endif
        }

        private readonly Container _DiContainer = new Container();

        private static bool _isRestartRequested;
    }
}