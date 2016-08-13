using Ann.Core;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarViewModelTest
    {
        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            using (new StatusBarViewModel(app))
            {
            }
        }

        [WpfFact]
        public void Messages()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                using (var vm = new StatusBarViewModel(app))
                {
                    Assert.Equal(0, vm.Messages.Count);
                    vm.Messages.Add(new ProcessingStatusBarItemViewModel("aaa"));
                    Assert.Equal(1, vm.Messages.Count);
                }
            }
        }

        [WpfFact]
        public void Visibility()
        {
            TestHelper.CleanTestEnv();

            using (var app = new App())
            {
                app.Config.TargetFolder.IsIncludeSystemFolder = false;
                app.Config.TargetFolder.IsIncludeSystemX86Folder = false;
                app.Config.TargetFolder.IsIncludeProgramsFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
                app.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
                app.Config.TargetFolder.IsIncludeCommonStartMenu = false;

                app.OpenIndexAsync().Wait();

                using (var vm = new StatusBarViewModel(app))
                {
                    Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);

                    var i = new ProcessingStatusBarItemViewModel("aaa");
                    vm.Messages.Add(i);
                    Assert.Equal(System.Windows.Visibility.Visible, vm.Visibility.Value);

                    vm.Messages.Remove(i);
                    Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);
                }
            }
        }
    }
}