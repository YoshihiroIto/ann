using System.Reflection;
using System.Windows;
using Ann.Core;
using Ann.SettingWindow.SettingPage;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class FileOrFolderListBoxTest
    {
        [WpfFact]
        public void Basic()
        {
            Application.ResourceAssembly = Assembly.GetAssembly(typeof(App));

            var e = new Entry();
            e.InitializeComponent();

            var c = new FileOrFolderListBox();
        }
    }
}