﻿using System;
using System.Runtime;
using System.Windows;
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
            ProfileOptimization.SetProfileRoot(App.ConfigDirPath);
            ProfileOptimization.StartProfile("Startup.Profile");

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
