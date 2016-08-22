using System;
using System.Linq;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarItemViewModelTest :IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public StatusBarItemViewModelTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Fact]
        public void StatusBarItemViewModel_Messages()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var messages = new[] {StringTags.AllFiles, StringTags.File};
                
                using (var vm = new StatusBarItemViewModel(app, messages))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void StatusBarItemViewModel_KeyOptions()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new StatusBarItemViewModel(
                    app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void StatusBarItemViewModel_Options()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new StatusBarItemViewModel(
                    app, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_Messages()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var messages = new[] {StringTags.AllFiles, StringTags.File};
                
                using (var vm = new ProcessingStatusBarItemViewModel(app, messages))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_KeyOptions()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new ProcessingStatusBarItemViewModel(
                    app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_Options()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new ProcessingStatusBarItemViewModel(
                    app, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_Messages()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var messages = new[] {StringTags.AllFiles, StringTags.File};
                
                using (var vm = new WaitingStatusBarItemViewModel(app, messages))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_KeyOptions()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new WaitingStatusBarItemViewModel(
                    app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_Options()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                using (var vm = new WaitingStatusBarItemViewModel(
                    app, StringTags.Download, new object[] {1,2,3}))
                {
                    Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                    Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                    Assert.Equal(new object[] {1,2,3}, vm.Messages.Value.First().Options);
                    Assert.Same(app, vm.App);
                }
            }
        }
    }
}