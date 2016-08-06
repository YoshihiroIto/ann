using Ann.Foundation.Mvvm;
using Xunit;


namespace Ann.Foundation.Test.Mvvm
{
    public class DisposableNotificationObjectTest
    {
        public class Model : DisposableNotificationObject
        {
        }

        [Fact]
        public void Basic()
        {
            using (new Model())
            {
            }
        }

        [Fact]
        public void UseCompositDisposable()
        {
            using (var model = new Model())
            {
                model.CompositeDisposable.Add(new Model());
            }
        }
    }
}
