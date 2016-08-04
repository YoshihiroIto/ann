using Ann.Foundation.Mvvm;
using Xunit;

namespace Ann.Foundation.Test.Mvvm
{
    public class ViewModelBaseTest
    {
        public class ViewModel : ViewModelBase
        {
            public ViewModel()
            {
            }

            public ViewModel(bool disableDisposableChecker = false)
                : base(disableDisposableChecker)
            {
            }
        }

        [Fact]
        public void Simple()
        {
            using (new ViewModel())
            {
            }
        }

        [Fact]
        public void UseCompositDisposable()
        {
            using (var viewModel = new ViewModel())
            {
                viewModel.CompositeDisposable.Add(new ViewModel());
            }
        }

#if DEBUG
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void UndisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            // ReSharper disable once UnusedVariable
            var viewModel = new ViewModel(disableDisposableChecker);

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void MultipleDisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            var viewModel = new ViewModel(disableDisposableChecker);

            viewModel.Dispose();
            viewModel.Dispose();

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }
#endif
    }
}
