using System.Reflection;
using System.Windows;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class FileOrFolderListBoxTest
    {
        [WpfFact]
        public void Basic()
        {
            RunOnTestDomain.Do(() =>
            {
                if (Application.ResourceAssembly == null)
                    Application.ResourceAssembly = Assembly.GetAssembly(typeof(Entry));

                var e = new Entry();
                e.InitializeComponent();

                var c = new FileOrFolderListBox();
            });
        }
    }
}