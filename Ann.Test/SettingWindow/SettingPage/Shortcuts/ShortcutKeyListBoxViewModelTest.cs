using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.Properties;
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


        [Fact]
        public void IsFocused_Empty_AutoRemove()
        {
            var model = new ObservableCollection<ShortcutKey>();

            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                model.Add(new ShortcutKey {Key = Key.A});

                Assert.Equal(1, vm.Keys.Count);

                vm.Keys[0].IsFocused.Value = true;

                vm.Keys[0].Key.Value = Key.None;

                vm.Keys[0].IsFocused.Value = false;

                Assert.Equal(0, model.Count);
            }
        }

        [Fact]
        public void Validate_AlreadySetSameKey()
        {
            var model = new ObservableCollection<ShortcutKey>();

            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                model.Add(new ShortcutKey {Key = Key.A});
                model.Add(new ShortcutKey {Key = Key.B});

                Assert.Null(vm.Keys[0].ValidationMessage.Value);
                Assert.Null(vm.Keys[1].ValidationMessage.Value);

                model.Add(new ShortcutKey {Key = Key.A});

                Assert.Equal(Resources.Message_AlreadySetSameKeyStroke, vm.Keys[0].ValidationMessage.Value);
                Assert.Null(vm.Keys[1].ValidationMessage.Value);
                Assert.Equal(Resources.Message_AlreadySetSameKeyStroke, vm.Keys[2].ValidationMessage.Value);
            }
        }
    }
}