using System;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MainWindowViewModelTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                Assert.True(double.IsNaN(vm.Left.Value));
                Assert.True(double.IsNaN(vm.Top.Value));
                Assert.False(double.IsNaN(vm.CandidatesListMaxHeight.Value));
                Assert.NotNull(vm.Messenger);
                Assert.Equal($"{AssemblyConstants.Product} {AssemblyConstants.Version}", vm.Caption);
                Assert.NotNull(vm.StatusBar);
            }
        }

        [Fact]
        public void SettingShowCommand()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                vm.SettingShowCommand.Execute(null);
            }
        }

        [Fact]
        public void ShowCommand()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                vm.ShowCommand.Execute(null);
            }
        }

        [Fact]
        public void HideCommand()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                vm.HideCommand.Execute(null);
            }
        }

        [Fact]
        public void ExitCommand()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                vm.ExitCommand.Execute(null);
            }
        }

        [Fact]
        public void Candidates()
        {
            using (var vm = _context.GetInstance<MainWindowViewModel>())
            {
                Assert.Empty(vm.Candidates.Value);
            }
        }
    }
}