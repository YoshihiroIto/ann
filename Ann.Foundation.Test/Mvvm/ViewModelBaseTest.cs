using Ann.Foundation.Mvvm;
using Xunit;

namespace Ann.Foundation.Test.Mvvm
{
    public class ViewModelBaseTest
    {
        public class ViewModel : ViewModelBase
        {
        }

        [Fact]
        public void Basic()
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
    }
}
