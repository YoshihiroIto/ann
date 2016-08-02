using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class AnonymousDisposableTest
    {
        [TestMethod]
        public void Simple()
        {
            var n = 0;
            var d = new AnonymousDisposable(() => n ++);

            Assert.AreEqual(n, 0);
            d.Dispose();
            Assert.AreEqual(n, 1);
        }

        [TestMethod]
        public void EmptyDisposing()
        {
            var r = new AnonymousDisposable();
            r.Dispose();
        }

        [TestMethod]
        public void MultipleDisposing()
        {
            var n = 0;
            var d = new AnonymousDisposable(() => n ++);

            Assert.AreEqual(n, 0);
            d.Dispose();
            Assert.AreEqual(n, 1);
            d.Dispose();
            Assert.AreEqual(n, 1);
        }

        [TestMethod]
        public void UsingBlock()
        {
            var i = 100;

            using (new AnonymousDisposable(() => i = 200))
            {
                Assert.AreEqual(i, 100);
                i = 300;
                Assert.AreEqual(i, 300);
            }

            Assert.AreEqual(i, 200);
        }
    }
}
