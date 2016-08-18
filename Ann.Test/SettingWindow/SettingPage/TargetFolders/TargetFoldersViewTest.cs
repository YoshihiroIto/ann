using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.TargetFolders
{
    public class TargetFoldersViewTest : MarshalByRefObject, IDisposable
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
                    // ReSharper disable once ObjectCreationAsStatement
                    new TargetFoldersView();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}