﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class ExecutableUnitViewModelTest : IDisposable
    { 
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _config.Dispose();
        }

        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var parent = new MainWindowViewModel(app, configHolder))
            using (var vm = new ExecutableUnitViewModel(parent, model, app))
            {
                Assert.Equal("Ann", vm.Name);
                Assert.Equal(path, vm.Path);
                Assert.NotNull(vm.Icon);
            }
        }

        [WpfFact]
        public void PriorityFile()
        {
            TestHelper.CleanTestEnv();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var parent = new MainWindowViewModel(app, configHolder))
            using (var vm = new ExecutableUnitViewModel(parent, model, app))
            {
                Assert.False(vm.IsPriorityFile);

                vm.IsPriorityFile = true;
                Assert.True(vm.IsPriorityFile);
                Assert.True(app.IsPriorityFile(path));

                vm.IsPriorityFile = false;
                Assert.False(vm.IsPriorityFile);
                Assert.False(app.IsPriorityFile(path));
            }
        }

        [WpfFact]
        public void IsPriorityFileFlipCommand()
        {
            TestHelper.CleanTestEnv();

            var path = Assembly.GetEntryAssembly().Location;
            var stringPool = new ConcurrentDictionary<string, string>();
            var targetFolders = new string[0];

            var model = new ExecutableUnit(path, stringPool, targetFolders);

            var configHolder = new ConfigHolder(_config.RootPath);
            using (var app = new App(configHolder))
            using (var parent = new MainWindowViewModel(app, configHolder))
            using (var vm = new ExecutableUnitViewModel(parent, model, app))
            {
                Assert.False(vm.IsPriorityFile);
                Assert.False(app.IsPriorityFile(path));

                vm.IsPriorityFileFlipCommand.Execute(null);

                Assert.True(vm.IsPriorityFile);
                Assert.True(app.IsPriorityFile(path));

                vm.IsPriorityFileFlipCommand.Execute(null);

                Assert.False(vm.IsPriorityFile);
                Assert.False(app.IsPriorityFile(path));
            }
        }
    }
}