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

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void UndisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            // ReSharper disable once UnusedVariable
            var model = new Model(disableDisposableChecker);

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

            var model = new Model(disableDisposableChecker);

            model.Dispose();
            model.Dispose();

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }
    }
}
