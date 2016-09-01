using System.Windows.Input;
using Ann.Core;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;
using App = Ann.Core.Config.App;

namespace Ann.Test.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutsViewModelTest
    {
        public ShortcutsViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }
        
        [Fact]
        public void Basic()
        {
            var model = new App();
            using (new ShortcutsViewModel(model))
            {
            }
        }

        [Fact]
        public void Activate()
        {
            var model = new App();
            using (var vm = new ShortcutsViewModel(model))
            {
                Assert.True(vm.Activate.IsControl.Value);
                Assert.False(vm.Activate.IsAlt.Value);
                Assert.False(vm.Activate.IsShift.Value);
                Assert.Equal(Key.Space, vm.Activate.Key.Value);
                Assert.Equal(ModifierKeys.Control, vm.Activate.Modifiers);
            }
        }

        [Fact]
        public void HideShortcutside()
        {
            var model = new App();
            using (var vm = new ShortcutsViewModel(model))
            {
                Assert.Equal(0, vm.HideShortcuts.Keys.Count);
                model.ShortcutKeys.Hide.Add(new ShortcutKey());
                Assert.Equal(1, vm.HideShortcuts.Keys.Count);
            }
        }
    }
}