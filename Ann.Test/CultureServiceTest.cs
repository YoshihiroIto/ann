using System.Windows.Threading;
using Ann.Foundation.Exception;
using Xunit;

namespace Ann.Test
{
    public class ViewManagerTestFixture
    {
        public ViewManagerTestFixture()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        public Dispatcher Dispatcher { get;  }
    }

    public class ViewManagerTest: IClassFixture<ViewManagerTestFixture>
    {
        private readonly Dispatcher _dispatcher;

        public ViewManagerTest(ViewManagerTestFixture fixture)
        {
            _dispatcher = fixture.Dispatcher;
        }

        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            ViewManager.Initialize(_dispatcher);

            Assert.Same(_dispatcher, ViewManager.Instance.UiDispatcher);

            ViewManager.Destory();
        }

        [Fact]
        public void NestingInitialize()
        {
            TestHelper.CleanTestEnv();

            ViewManager.Initialize(_dispatcher);

            Assert.Throws<NestingException>(() =>
                ViewManager.Initialize(_dispatcher));
        }

        [Fact]
        public void NestingDestory()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<NestingException>(() =>
                ViewManager.Destory());
        }

        [Fact]
        public void Uninitialized()
        {
            TestHelper.CleanTestEnv();

            Assert.Throws<UninitializedException>(() =>
                ViewManager.Instance);
        }
    }
}