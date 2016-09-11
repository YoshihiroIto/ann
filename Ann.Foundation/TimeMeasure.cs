using System;
using System.Diagnostics;

namespace Ann.Foundation
{
    public class TimeMeasure : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _title;

        public TimeMeasure(string title = "")
        {
            _title = string.IsNullOrEmpty(title) ? nameof(TimeMeasure) : title;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        #region IDisposable
        private bool _isDisposed;

        ~TimeMeasure()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _stopwatch.Stop();
                Debug.WriteLine($"■{_title} : {_stopwatch.ElapsedMilliseconds}ms : {_stopwatch.ElapsedTicks}");
            }

            _isDisposed = true;
        }
        #endregion
    }
}