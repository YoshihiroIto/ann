using System;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows;
using Ann.Core;
using Ann.Foundation;
using Squirrel;

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
            ProfileOptimization.SetProfileRoot(ConfigHelper.ConfigDirPath);
            ProfileOptimization.StartProfile("Startup.Profile");

            Task.Run(async () =>
            {
                using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/YoshihiroIto/ann"))
                {
                    await mgr.Result.UpdateApp();
                }
            });

            DisposableChecker.Start();
            {
                var e = new Entry();
                e.InitializeComponent();
                e.Run();

                App.Destory();
            }
            DisposableChecker.End();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Livet.DispatcherHelper.UIDispatcher = Dispatcher;
            Reactive.Bindings.UIDispatcherScheduler.Initialize();

            App.Initialize();
        }
    }
}
