using System.Windows.Input;
using Ann.Core;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;
using App = Ann.Core.App;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class ShortcutsViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (new ShortcutsViewModel(app))
            {
            }

            App.Destory();
        }

        [Fact]
        public void Activate()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (var vm = new ShortcutsViewModel(app))
            {
                Assert.False(vm.Activate.IsControl.Value);
                Assert.False(vm.Activate.IsAlt.Value);
                Assert.False(vm.Activate.IsShift.Value);
                Assert.Equal(Key.None, vm.Activate.Key.Value);
                Assert.Equal(ModifierKeys.None, vm.Activate.Modifiers);
            }

            App.Destory();
        }

        [Fact]
        public void HideShortcutside()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();

            var app = new Core.Config.App();
            using (var vm = new ShortcutsViewModel(app))
            {
                Assert.Equal(0, vm.HideShortcuts.Keys.Count);
                app.ShortcutKeys.Hide.Add(new ShortcutKey());
                Assert.Equal(1, vm.HideShortcuts.Keys.Count);
            }

            App.Destory();
        }
    }
}