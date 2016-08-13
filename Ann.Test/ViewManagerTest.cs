using System.Windows.Threading;
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

            using ( new ViewManager(_dispatcher))
            {
            }
        }
    }
}