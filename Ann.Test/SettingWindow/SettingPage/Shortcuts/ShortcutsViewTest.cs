using System;
using System.Reflection;
using System.Windows;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutsViewTest : MarshalByRefObject, IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
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
                    new ShortcutsView();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}