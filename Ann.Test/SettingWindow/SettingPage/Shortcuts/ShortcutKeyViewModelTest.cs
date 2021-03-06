﻿using System;
using System.Windows.Input;
using Ann.Core;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutKeyViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var model = new ShortcutKey();
            using (var vm = new ShortcutKeyViewModel(model))
            {
                Assert.False(vm.IsControl.Value);
                Assert.False(vm.IsAlt.Value);
                Assert.False(vm.IsShift.Value);
                Assert.Equal(Key.None, vm.Key.Value);
                Assert.Equal(ModifierKeys.None, vm.Modifiers);

                vm.IsAlt.Value = true;
                Assert.True(vm.IsAlt.Value);
                Assert.True(model.Modifiers == ModifierKeys.Alt);
                Assert.True(vm.Modifiers == ModifierKeys.Alt);

                vm.IsControl.Value = true;
                Assert.True(vm.IsControl.Value);
                Assert.True(model.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt));
                Assert.True(vm.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt));

                vm.IsShift.Value = true;
                Assert.True(vm.IsShift.Value);
                Assert.True(model.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift));
                Assert.True(vm.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift));

                model.Key = Key.A;
                Assert.Equal(Key.A, model.Key);
                Assert.Equal(Key.A, vm.Key.Value);
            }
        }
    }
}