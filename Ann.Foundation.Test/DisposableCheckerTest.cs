using System;
using Xunit;

namespace Ann.Foundation.Test
{
    public class DisposableCheckerTest : IDisposable
    {
        public DisposableCheckerTest()
        {
            DisposableChecker.Clean();
        }

        public void Dispose()
        {
            DisposableChecker.Clean();
        }

        private class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        [Fact]
        public void Basic()
        {
            var message = "ABC";

            DisposableChecker.Start(m => message = m);

            var d = new Disposable();
            DisposableChecker.Add(d);
            DisposableChecker.Remove(d);
            d.Dispose();

            DisposableChecker.End();

            Assert.Equal("ABC", message);
        }

        [Fact]
        public void NullStart()
        {
            DisposableChecker.Start(null);
            DisposableChecker.End();
        }

#if DEBUG
        [Fact]
        public void NestingStart()
        {
            DisposableChecker.Start(null);

            Assert.Throws<NestingException>(() =>
            {
                DisposableChecker.Start(null);
            });

            DisposableChecker.Clean();
        }

        [Fact]
        public void NestingEnd()
        {
            Assert.Throws<NestingException>(() =>
            {
                DisposableChecker.End();
            });

            DisposableChecker.Clean();
        }

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

            Assert.Equal("ABC", message);
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

            Assert.Equal("ABC", message);
            DisposableChecker.Remove(d);

            Assert.Equal("ABC", message);
            DisposableChecker.Remove(d);

            Assert.True(message.Contains("Found multiple removing."));

            DisposableChecker.End();
        }

        public class Model : DisposableNotificationObject
        {
            public Model(bool disableDisposableChecker = false)
                : base(disableDisposableChecker)
            {
            }
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void DisposableNotificationObjectUndisposingCheck(bool expected, bool disableDisposableChecker)
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
        public void DisposableNotificationObjectMultipleDisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            var model = new Model(disableDisposableChecker);

            model.Dispose();
            model.Dispose();

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }

        public class ViewModel : ViewModelBase
        {
            public ViewModel(bool disableDisposableChecker = false)
                : base(disableDisposableChecker)
            {
            }
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ViewModelBaseUndisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            // ReSharper disable once UnusedVariable
            var viewModel = new ViewModel(disableDisposableChecker);

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ViewModelBaseMultipleDisposingCheck(bool expected, bool disableDisposableChecker)
        {
            var message = string.Empty;

            DisposableChecker.Start(m => message = m);

            var viewModel = new ViewModel(disableDisposableChecker);

            viewModel.Dispose();
            viewModel.Dispose();

            DisposableChecker.End();

            Assert.Equal(expected, message.Contains("Found"));
        }
#endif
    }
}
