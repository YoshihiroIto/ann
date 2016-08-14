using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class FileOrFolderListBoxTest: IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
#if false
            RunOnTestDomain.Do(() =>
            {
                if (Application.ResourceAssembly == null)
                    Application.ResourceAssembly = Assembly.GetAssembly(typeof(Entry));

                var e = new Entry();
                e.InitializeComponent();

                var c = new FileOrFolderListBox();
            });
#endif
        }
    }
}