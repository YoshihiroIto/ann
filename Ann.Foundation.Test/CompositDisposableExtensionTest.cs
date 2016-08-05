using System.Reactive.Disposables;
using Xunit;

namespace Ann.Foundation.Test
{
    public class CompositDisposableExtensionTest
    {
        [Fact]
        public void Action()
        {
            var i = 100;

            Assert.Equal(100, i);

            using (var cd = new CompositeDisposable())
            {
                Assert.Equal(100, i);

                cd.Add(() => i ++);

                Assert.Equal(100, i);
            }

            Assert.Equal(101, i);
        }
    }
}