using System.Windows.Input;
using Ann.Core.Config;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Xunit;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class ShortcutsViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var model = new App();
            using (new ShortcutsViewModel(model))
            {
            }
        }

        [Fact]
        public void Activate()
        {
            TestHelper.CleanTestEnv();

            var model = new App();
            using (var vm = new ShortcutsViewModel(model))
            {
                Assert.False(vm.Activate.IsControl.Value);
                Assert.False(vm.Activate.IsAlt.Value);
                Assert.False(vm.Activate.IsShift.Value);
                Assert.Equal(Key.None, vm.Activate.Key.Value);
                Assert.Equal(ModifierKeys.None, vm.Activate.Modifiers);
            }
        }

        [Fact]
        public void HideShortcutside()
        {
            TestHelper.CleanTestEnv();

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