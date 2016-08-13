using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class ShortcutKeyListBoxViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

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
            TestHelper.CleanTestEnv();

            var model = new ObservableCollection<ShortcutKey>
            {
                new ShortcutKey {Key = Key.A},
                new ShortcutKey {Key = Key.B},
                new ShortcutKey {Key = Key.C}
            };

            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                Assert.Equal(3, model.Count);

                vm.KeyRemoveCommand.Execute(new ShortcutKeyViewModel(new ShortcutKey {Key = Key.B}));

                Assert.Equal(2, model.Count);
                Assert.Equal(Key.A, model[0].Key);
                Assert.Equal(Key.C, model[1].Key);
            }
        }
    }
}