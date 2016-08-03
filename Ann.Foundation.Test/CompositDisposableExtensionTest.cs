using System.Reactive.Disposables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class CompositDisposableExtensionTest
    {
        [TestMethod]
        public void Action()
        {
            var i = 100;

            Assert.AreEqual(i, 100);

            using (var cd = new CompositeDisposable())
            {
                Assert.AreEqual(i, 100);

                cd.Add(() => i ++);

                Assert.AreEqual(i, 100);
            }

            Assert.AreEqual(i, 101);
        }
    }
}