using System.Reactive.Concurrency;
using System.Reflection;
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

            if (Assembly.GetEntryAssembly() == null)
                Foundation.TestHelper.SetEntryAssembly();

            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
        }
    }
}