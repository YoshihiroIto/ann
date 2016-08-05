using Ann.Foundation.Mvvm;
using Xunit;


namespace Ann.Foundation.Test.Mvvm
{
    public class DisposableNotificationObjectTest
    {
        public class Model : DisposableNotificationObject
        {
            public Model()
            {
            }

            public Model(bool disableDisposableChecker = false)
                : base(disableDisposableChecker)
            {
            }
        }

        [Fact]
        public void Simple()
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
