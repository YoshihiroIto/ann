using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Threading;
using Ann.Foundation;
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