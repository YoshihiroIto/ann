using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Core;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;
using App = Ann.Core.App;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class ShortcutKeyListBoxViewModelTest
    {
        [Fact]
        public void Basic()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var model = new ObservableCollection<ShortcutKey>();
            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                Assert.Equal(0, vm.Keys.Count);
                model.Add(new ShortcutKey());
                Assert.Equal(1, vm.Keys.Count);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void KeyAddCommand()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var model = new ObservableCollection<ShortcutKey>();
            using (var vm = new ShortcutKeyListBoxViewModel(model))
            {
                vm.KeyAddCommand.Execute(null);
                Assert.Equal(1, vm.Keys.Count);
                Assert.Equal(1, model.Count);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void KeyRemoveCommand()
        {
            App.Initialize();
            VersionUpdater.Initialize();

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

            VersionUpdater.Destory();
            App.Destory();
            
        }
    }
}