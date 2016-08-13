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

                CultureService.Destory();
            }
            DisposableChecker.End();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _viewManager = new ViewManager(Dispatcher);
            ConfigHolder = new ConfigHolder(Constants.ConfigDirPath);

            App = new App(ConfigHolder);

            CultureService.Initialize(ConfigHolder.Config);
            Reactive.Bindings.UIDispatcherScheduler.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            App.Dispose();
            _viewManager.Dispose();
        }

        public static App App { get; private set; }
        public static ConfigHolder ConfigHolder { get; private set; }

        private ViewManager _viewManager;
    }
}
