using System;
using Ann.Core;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [WpfFact]
        public void Basic()
        {
            var app = _context.GetInstance<App>();
            
            using (new StatusBarViewModel(app))
            {
            }
        }

        [WpfFact]
        public void Messages()
        {
            var app = _context.GetInstance<App>();
            var configHolder = _context.GetInstance<ConfigHolder>();

            {
                configHolder.Config.TargetFolder.IsIncludeSystemFolder = false;
                configHolder.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramsFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                configHolder.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                configHolder.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                using (var vm = new StatusBarViewModel(app))
                {
                    Assert.Equal(0, vm.Messages.Count);
                    vm.Messages.Add(new ProcessingStatusBarItemViewModel(app, StringTags.AllFiles));
                    Assert.Equal(1, vm.Messages.Count);
                }
            }
        }
    }
}