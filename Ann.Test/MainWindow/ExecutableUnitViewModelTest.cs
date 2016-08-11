using System.Collections.Concurrent;
using System.Reflection;
using Ann.Core;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class ExecutableUnitViewModelTest
    {
        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();
            App.Initialize();
            VersionUpdater.Initialize();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            using (var parent = new MainWindowViewModel())
            using (var vm = new ExecutableUnitViewModel(parent, model))
            {
                Assert.Equal("Ann", vm.Name);
                Assert.Equal(path, vm.Path);
                Assert.NotNull(vm.Icon);
            }

            VersionUpdater.Destory();
            App.Destory();
        }

        [WpfFact]
        public void PriorityFile()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();
            VersionUpdater.Initialize();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            using (var parent = new MainWindowViewModel())
            using (var vm = new ExecutableUnitViewModel(parent, model))
            {
                Assert.False(vm.IsPriorityFile);

                vm.IsPriorityFile = true;
                Assert.True(vm.IsPriorityFile);
                Assert.True(App.Instance.IsPriorityFile(path));

                vm.IsPriorityFile = false;
                Assert.False(vm.IsPriorityFile);
                Assert.False(App.Instance.IsPriorityFile(path));
            }

            VersionUpdater.Destory();
            App.Destory();
        }


        [WpfFact]
        public void IsPriorityFileFlipCommand()
        {
            TestHelper.CleanTestEnv();

            App.Initialize();
            VersionUpdater.Initialize();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            using (var parent = new MainWindowViewModel())
            using (var vm = new ExecutableUnitViewModel(parent, model))
            {
                Assert.False(vm.IsPriorityFile);
                Assert.False(App.Instance.IsPriorityFile(path));

                vm.IsPriorityFileFlipCommand.Execute(null);

                Assert.True(vm.IsPriorityFile);
                Assert.True(App.Instance.IsPriorityFile(path));

                vm.IsPriorityFileFlipCommand.Execute(null);

                Assert.False(vm.IsPriorityFile);
                Assert.False(App.Instance.IsPriorityFile(path));
            }

            VersionUpdater.Destory();
            App.Destory();
        }

    }
}