using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Reactive.Bindings;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutkeyListBoxTest : MarshalByRefObject, IDisposable
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
                    var c = new ShortcutkeyListBox();

                    using (var command = new ReactiveCommand())
                    {
                        c.AddCommand = command;
                        c.RemoveCommand = command;

                        Assert.Same(command, c.AddCommand);
                        Assert.Same(command, c.RemoveCommand);
                    }

                    var l = new ObservableCollection<int>();
                    c.Items = l;

                    Assert.Same(l, c.Items);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}