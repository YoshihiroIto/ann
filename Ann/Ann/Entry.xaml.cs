﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Windows;

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
            ProfileOptimization.SetProfileRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            ProfileOptimization.StartProfile("Startup.Profile");

            var e = new Entry();
            e.InitializeComponent();
            e.Run();

            App.Destory();
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
