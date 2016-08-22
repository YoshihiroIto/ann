using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.PriorityFiles
{
    public class PriorityFilesViewTest : MarshalByRefObject, IDisposable
    {
        public void Dispose()
        {
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
                    new PriorityFilesView();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}