
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
    
namespace Ann.Foundation.Test
{
    [TestClass]
    public class DisposableCheckerTest
    {
        private class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

#if DEBUG
        [TestMethod]
        public void Simple()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            DisposableChecker.Remove(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.AreEqual(message, "ABC");
        }

        [TestMethod]
        public void NullStart()
        {
            DisposableChecker.Start(null);
            DisposableChecker.End();
        }

        [TestMethod]
        public void Undispose()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.IsTrue(message.Contains("Found undispose object."));
        }

        [TestMethod]
        public void MultipleAddition()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            Assert.AreEqual(message, "ABC");
            DisposableChecker.Add(d);

            Assert.IsTrue(message.Contains("Found multiple addition."));

            DisposableChecker.End();
        }

        [TestMethod]
        public void MultipleRemoving()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            Assert.AreEqual(message, "ABC");
            DisposableChecker.Remove(d);

            Assert.AreEqual(message, "ABC");
            DisposableChecker.Remove(d);

            Assert.IsTrue(message.Contains("Found multiple removing."));

            DisposableChecker.End();
        }
#else
        [TestMethod]
        public void Simple()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            DisposableChecker.Remove(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.AreEqual(message, "ABC");
        }

        [TestMethod]
        public void NullStart()
        {
            DisposableChecker.Start(null);
            DisposableChecker.End();
        }
#endif
    }
}
