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

        public void Dispose()
        {
            _stopwatch.Stop();
            Debug.WriteLine($"■{_title} : {_stopwatch.ElapsedMilliseconds}ms : {_stopwatch.ElapsedTicks}");
        }
    }
}