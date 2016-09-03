using System;
using Ann.Core;
using Ann.SettingWindow;
using Ann.SettingWindow.SettingPage.General;
using Xunit;

namespace Ann.Test.SettingWindow
{
    public class SettingViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var model = new Core.Config.App();
            var app = _context.GetInstance<App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                Assert.Equal(5, vm.Pages.Length);
                Assert.IsType<GeneralViewModel>(vm.SelectedPage.Value);
            }
        }

        [Fact]
        public void InitializeCommand()
        {
            var model = new Core.Config.App();
            var app = _context.GetInstance<App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.InitializeCommand.Execute(null);
            }
        }

        [Fact]
        public void CloseCommand()
        {
            var model = new Core.Config.App();
            var app = _context.GetInstance<App>();
            var versionUpdater = _context.GetInstance<VersionUpdater>();

            using (var vm = new SettingViewModel(model, versionUpdater, app))
            {
                vm.CloseCommand.Execute(null);
            }
        }
    }
}