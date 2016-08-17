using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ann.Foundation.Control.Behavior;
using Xunit;

namespace Ann.Foundation.Test.Control.Behavior
{
    public class ListBoxLastItemFocusOnItemAddedBehaviorTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void Basic()
        {
            var w = new Window();

            var listBox = new ListBox();
            w.Content = listBox;

            var b = new ListBoxLastItemFocusOnItemAddedBehavior {ItemType = typeof(TextBox)};

            b.Attach(listBox);

            w.Show();

            var source = new ObservableCollection<FrameworkElement>();

            listBox.ItemsSource = source;

            var abc = new TextBox {Text = "ABC"};
            var def = new TextBox {Text = "DEF"};

            Assert.False(abc.IsFocused);
            Assert.False(def.IsFocused);

            source.Add(abc);
            Assert.True(abc.IsFocused);

            source.Add(def);
            Assert.False(abc.IsFocused);
            Assert.True(def.IsFocused);

            w.Close();

            b.Detach();
        }
    }
}