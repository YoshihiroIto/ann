﻿using System;
using Ann.Core;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class PathViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            using (var vm = new PathViewModel(new Path("AA"), a => string.Empty))
            {
                Assert.Equal("AA", vm.Path.Value);
            }
        }

        [Fact]
        public void FolderSelectDialogOpenCommand()
        {
            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => _.Response = "123"))
            {
                using (var vm = new PathViewModel(new Path("AA"), a => null))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("AA", vm.Path.Value);
                }
            }

            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => _.Response = "123"))
            {
                using (var vm = new PathViewModel(new Path("AA"), a => "456"))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("456", vm.Path.Value);
                }
            }

            using (var messenger = new WindowMessageBroker())
            using (messenger.Subscribe<FileOrFolderSelectMessage>(_ => {}))
            {
                using (var vm = new PathViewModel(new Path("AA"), a => "XYZ"))
                {
                    vm.FolderSelectDialogOpenCommand.Execute(null);
                    Assert.Equal("XYZ", vm.Path.Value);
                }
            }
        }
    }
}