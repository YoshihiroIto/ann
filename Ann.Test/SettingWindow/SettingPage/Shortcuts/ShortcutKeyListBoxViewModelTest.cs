﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutKeyListBoxViewModelTest
    {
        public ShortcutKeyListBoxViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void Basic()
        {
            var model = new ObservableCollection<ShortcutKey>();

            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                Assert.Equal(0, vm.Keys.Count);
                model.Add(new ShortcutKey());
                Assert.Equal(1, vm.Keys.Count);
            }
        }

        [Fact]
        public void KeyAddCommand()
        {
            var model = new ObservableCollection<ShortcutKey>();
            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                vm.KeyAddCommand.Execute(null);
                Assert.Equal(1, vm.Keys.Count);
                Assert.Equal(1, model.Count);
            }
        }

        [Fact]
        public void KeyRemoveCommand()
        {
            var model = new ObservableCollection<ShortcutKey>
            {
                new ShortcutKey {Key = Key.A},
                new ShortcutKey {Key = Key.B},
                new ShortcutKey {Key = Key.C}
            };

            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                Assert.Equal(3, model.Count);

                vm.KeyRemoveCommand.Execute(vm.Keys[1]);

                Assert.Equal(2, model.Count);
                Assert.Equal(Key.A, model[0].Key);
                Assert.Equal(Key.C, model[1].Key);
            }
        }
    }
}