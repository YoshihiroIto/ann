using System;
using System.Diagnostics;

namespace Ann.Foundation
{
    public class TimeMeasure : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        public TimeMeasure()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            Debug.WriteLine($"TimeMeasure : {_stopwatch.ElapsedMilliseconds}ms : {_stopwatch.ElapsedTicks}");
        }
    }
}