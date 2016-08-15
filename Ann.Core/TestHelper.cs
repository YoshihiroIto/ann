using System.Reactive.Concurrency;
using System.Threading;
using Reactive.Bindings;

namespace Ann.Core
{
    public static class TestHelper
    {
        private static int _isCleaned;

        public static void CleanTestEnv()
        {
            if (Interlocked.Increment(ref _isCleaned) != 1)
                return;

            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
        }
    }
}