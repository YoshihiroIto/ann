using System;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Ann.Core;
using Ann.Core.Config;
using Ann.Foundation;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Reactive.Bindings;
using Xunit;
using App = Ann.Core.App;

namespace Ann.Test.SettingWindow.SettingPage
{
    public class ShortcutsTestFixture : IDisposable
    {
        public ShortcutsTestFixture()
        {
            TestHelper.SetEntryAssembly();
            App.Clean();
            VersionUpdater.Clean();
            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
        }

        public void Dispose()
        {
            VersionUpdater.Clean();
        }
    }

    public class ShortcutsViewModelTest : IClassFixture<ShortcutsTestFixture>,  IDisposable
    {
        // ReSharper disable once UnusedParameter.Local
        public ShortcutsViewModelTest(ShortcutsTestFixture f)
        {
            App.Clean();
            VersionUpdater.Clean();
        }

        public void Dispose()
        {
            App.Clean();
            VersionUpdater.Clean();
        }

        [Fact]
        public void Basic()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (new ShortcutsViewModel(app))
            {
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void Activate()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new ShortcutsViewModel(app))
            {
                Assert.False(vm.Activate.IsControl.Value);
                Assert.False(vm.Activate.IsAlt.Value);
                Assert.False(vm.Activate.IsShift.Value);
                Assert.Equal(Key.None, vm.Activate.Key.Value);
                Assert.Equal(ModifierKeys.None, vm.Activate.Modifiers);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [Fact]
        public void HideShortcutside()
        {
            App.Initialize();
            VersionUpdater.Initialize();

            var app = new Core.Config.App();
            using (var vm = new ShortcutsViewModel(app))
            {
                Assert.Equal(0, vm.HideShortcuts.Keys.Count);
                app.ShortcutKeys.Hide.Add(new ShortcutKey());
                Assert.Equal(1, vm.HideShortcuts.Keys.Count);
            }

            VersionUpdater.Destory();
            App.Destory();
        }
    }
}