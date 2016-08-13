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
                App.Destory();
            }
            DisposableChecker.End();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            App.Initialize();
            CultureService.Initialize(App.Instance.Config);

            _viewManager = new ViewManager(Dispatcher);
            Reactive.Bindings.UIDispatcherScheduler.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _viewManager.Dispose();
        }

        private ViewManager _viewManager;
    }
}
