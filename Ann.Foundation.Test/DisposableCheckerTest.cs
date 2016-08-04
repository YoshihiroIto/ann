using System;
using Xunit;

namespace Ann.Foundation.Test
{
    public class DisposableCheckerTest
    {
        private class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        [Fact]
        public void Simple()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            DisposableChecker.Remove(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.Equal(message, "ABC");
        }

        [Fact]
        public void NullStart()
        {
            DisposableChecker.Start(null);
            DisposableChecker.End();
        }

#if DEBUG
        [Fact]
        public void Undispose()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.True(message.Contains("Found undispose object."));
        }

        [Fact]
        public void MultipleAddition()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            Assert.Equal(message, "ABC");
            DisposableChecker.Add(d);

            Assert.True(message.Contains("Found multiple addition."));

            DisposableChecker.End();
        }

        [Fact]
        public void MultipleRemoving()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            d.Dispose();

            Assert.Equal(message, "ABC");
            DisposableChecker.Remove(d);

            Assert.Equal(message, "ABC");
            DisposableChecker.Remove(d);

            Assert.True(message.Contains("Found multiple removing."));

            DisposableChecker.End();
        }
#endif
    }
}
