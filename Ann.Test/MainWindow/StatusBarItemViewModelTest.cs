using System;
using System.Linq;
using Ann.Core;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarItemViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void StatusBarItemViewModel_Messages()
        {
            var app = _context.GetInstance<App>();

            var messages = new[] {StringTags.AllFiles, StringTags.File};

            using (var vm = new StatusBarItemViewModel(app, messages))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void StatusBarItemViewModel_KeyOptions()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new StatusBarItemViewModel(
                app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void StatusBarItemViewModel_Options()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new StatusBarItemViewModel(
                app, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_Messages()
        {
            var app = _context.GetInstance<App>();

            var messages = new[] {StringTags.AllFiles, StringTags.File};

            using (var vm = new ProcessingStatusBarItemViewModel(app, messages))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_KeyOptions()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new ProcessingStatusBarItemViewModel(
                app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void ProcessingStatusBarItemViewModel_Options()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new ProcessingStatusBarItemViewModel(
                app, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_Messages()
        {
            var app = _context.GetInstance<App>();

            var messages = new[] {StringTags.AllFiles, StringTags.File};

            using (var vm = new WaitingStatusBarItemViewModel(app, messages))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(messages, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {null, null}, vm.Messages.Value.Select(x => x.Options));
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_KeyOptions()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new WaitingStatusBarItemViewModel(
                app, StatusBarItemViewModel.SearchKey.InOpening, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }

        [Fact]
        public void WaitingStatusBarItemViewModel_Options()
        {
            var app = _context.GetInstance<App>();

            using (var vm = new WaitingStatusBarItemViewModel(
                app, StringTags.Download, new object[] {1, 2, 3}))
            {
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
                Assert.Equal(new[] {StringTags.Download}, vm.Messages.Value.Select(x => x.String));
                Assert.Equal(new object[] {1, 2, 3}, vm.Messages.Value.First().Options);
                Assert.Same(app, vm.App);
            }
        }
    }
}