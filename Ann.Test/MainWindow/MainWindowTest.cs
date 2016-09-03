using System;
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
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();

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
                    _context.GetInstance<Ann.MainWindow.MainWindow>();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}