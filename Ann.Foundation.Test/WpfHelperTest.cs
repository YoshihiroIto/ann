﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Xunit;

namespace Ann.Foundation.Test
{
    public class WpfHelperTest : IDisposable
    {
        public void Dispose()
        {
            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [WpfFact]
        public void IsDesignMode()
        {
            Assert.False(WpfHelper.IsDesignMode);
        }

        [WpfFact]
        public void DoEvents()
        {
            // 例外にならない
            WpfHelper.DoEvents();
        }

        [WpfFact]
        public void FindChild()
        {
            {
                var w = new Window();

                var b = WpfHelper.FindChild(w, typeof(Button));

                Assert.Null(b);

                w.Close();
            }

            {
                var b = WpfHelper.FindChild(null, typeof(Button));

                Assert.Null(b);
            }

            {
                var w = new Window();

                var b = WpfHelper.FindChild(w, typeof(Window));

                Assert.Same(w, b);

                w.Close();
            }

            {
                var w = new Window();

                var sb = new Button();
                var s = new StackPanel();
                s.Children.Add(new ListBox());
                s.Children.Add(new Label());
                s.Children.Add(sb);
                w.Content = s;

                var b = WpfHelper.FindChild(w, typeof(Button));

                Assert.Same(sb, b);

                w.Close();
            }
        }

        [WpfFact]
        public void FindChildGeneric()
        {
            {
                var w = new Window();

                var b = WpfHelper.FindChild<Button>(w);

                Assert.Null(b);

                w.Close();
            }

            {
                var b = WpfHelper.FindChild<Button>(null);

                Assert.Null(b);
            }

            {
                var w = new Window();

                var b = WpfHelper.FindChild<Window>(w);

                Assert.Same(w, b);

                w.Close();
            }

            {
                var w = new Window();

                var sb = new Button();
                var s = new StackPanel();
                s.Children.Add(new ListBox());
                s.Children.Add(new Label());
                s.Children.Add(sb);
                w.Content = s;

                var b = WpfHelper.FindChild<Button>(w);

                Assert.Same(sb, b);

                w.Close();
            }
        }
    }
}
