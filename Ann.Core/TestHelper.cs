using System.Reactive.Concurrency;
using System.Reflection;
using Reactive.Bindings;

namespace Ann.Core
{
    public static class TestHelper
    {
        public static void CleanTestEnv()
        {
            if (Assembly.GetEntryAssembly() == null)
                Foundation.TestHelper.SetEntryAssembly();

            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
        }
    }
}