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
            VersionUpdater.Initialize();
            {
                var e = new Entry();
                e.InitializeComponent();
                e.Run();

                ViewManager.Destory();
                CultureService.Destory();
                App.Destory();
            }
            VersionUpdater.Destory();
            DisposableChecker.End();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            App.Initialize();
            CultureService.Initialize(App.Instance.Config);
            ViewManager.Initialize(Dispatcher);

            Reactive.Bindings.UIDispatcherScheduler.Initialize();
        }
    }
}
