using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class StatusBarItemViewModelTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            using (var vm = new StatusBarItemViewModel("ABC"))
            {
                Assert.Equal("ABC", vm.Message.Value);
                Assert.Equal(StatusBarItemViewModel.SearchKey.NoKey, vm.Key);
            }

            using (var vm = new StatusBarItemViewModel(StatusBarItemViewModel.SearchKey.InOpening, "DEF"))
            {
                Assert.Equal("DEF", vm.Message.Value);
                Assert.Equal(StatusBarItemViewModel.SearchKey.InOpening, vm.Key);
            }
        }
    }
}