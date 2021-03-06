﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using Ann.Core;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage;
using Reactive.Bindings;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class FileOrFolderListBoxTest : MarshalByRefObject, IDisposable
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
                    var c = new FileOrFolderListBox();

                    var dummyCmd = new ReactiveCommand();

                    Assert.Null(c.AddCommand);
                    c.AddCommand = dummyCmd;
                    Assert.Same(dummyCmd, c.AddCommand);

                    Assert.Null(c.RemoveCommand);
                    c.RemoveCommand = dummyCmd;
                    Assert.Same(dummyCmd, c.RemoveCommand);

                    var l = new List<string>();
                    Assert.Null(c.Items);
                    c.Items = l;
                    Assert.Same(l, c.Items);

                    Assert.Null(c.AddButtonText);
                    c.AddButtonText = "AAA";
                    Assert.Equal("AAA", c.AddButtonText);

                    Assert.False(c.IsFolderPicker);
                    c.IsFolderPicker = true;
                    Assert.True(c.IsFolderPicker);

                    dummyCmd.Dispose();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }

        [WpfFact]
        public void ItemsAdd()
        {
            RunOnTestDomain.Do(() =>
            {
                Application.ResourceAssembly = Assembly.GetAssembly(typeof(Entry));

                var entry = new Entry();
                entry.InitializeComponent();

                try
                {
                    var c = new FileOrFolderListBox();

                    // todo:テストを実装する

                    var l = new ObservableCollection<string>();

                    c.Items = l;

                    l.Add("AAA");
                    l.Add("BBB");
                    l.Add("CCC");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + e.StackTrace);
                }
            });
        }
    }
}