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

            Assert.Equal(i, 100);

            using (var cd = new CompositeDisposable())
            {
                Assert.Equal(i, 100);

                cd.Add(() => i ++);

                Assert.Equal(i, 100);
            }

            Assert.Equal(i, 101);
        }
    }
}