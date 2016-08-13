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
            App.Initialize();

            using (new StatusBarViewModel())
            {
            }

            App.Destory();
        }

        [WpfFact]
        public void Messages()
        {
            TestHelper.CleanTestEnv();
            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            App.Instance.OpenIndexAsync().Wait();

            using (var vm = new StatusBarViewModel())
            {
                Assert.Equal(0, vm.Messages.Count);
                vm.Messages.Add(new ProcessingStatusBarItemViewModel("aaa"));
                Assert.Equal(1, vm.Messages.Count);
            }

            App.Destory();
        }

        [WpfFact]
        public void Visibility()
        {
            TestHelper.CleanTestEnv();
            App.Initialize();

            App.Instance.Config.TargetFolder.IsIncludeSystemFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeSystemX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramsFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesFolder = false;
            App.Instance.Config.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            App.Instance.Config.TargetFolder.IsIncludeCommonStartMenu = false;

            App.Instance.OpenIndexAsync().Wait();

            using (var vm = new StatusBarViewModel())
            {
                Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);

                var i = new ProcessingStatusBarItemViewModel("aaa");
                vm.Messages.Add(i);
                Assert.Equal(System.Windows.Visibility.Visible, vm.Visibility.Value);

                vm.Messages.Remove(i);
                Assert.Equal(System.Windows.Visibility.Collapsed, vm.Visibility.Value);
            }

            App.Destory();
        }
    }
}