﻿using System;
using System.Runtime;
using System.Threading.Tasks;
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
            ProfileOptimization.SetProfileRoot(ConfigHelper.ConfigDirPath);
            ProfileOptimization.StartProfile("Startup.Profile");

            VersionUpdater.Initialize();
            DisposableChecker.Start();
            {
                Task.Run(async () => await VersionUpdater.Instance.UpdateApp());

                var e = new Entry();
                e.InitializeComponent();
                e.Run();

                ViewManager.Destory();
                App.Destory();
            }
            DisposableChecker.End();
            VersionUpdater.Destory();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            App.Initialize();
            ViewManager.Initialize(Dispatcher);

            Reactive.Bindings.UIDispatcherScheduler.Initialize();
        }
    }
}
