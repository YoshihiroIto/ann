﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MainWindowTest : MarshalByRefObject, IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _config.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            RunOnTestDomain.Do(() =>
            {
                Application.ResourceAssembly = Assembly.GetAssembly(typeof(Entry));

                var entry = new Entry();
                entry.InitializeComponent();

                try
                {
                    var configHolder = new ConfigHolder(_config.RootPath);
                    using (var app = new App(configHolder))
                    {
                        var c = new Ann.MainWindow.MainWindow(app, configHolder);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}