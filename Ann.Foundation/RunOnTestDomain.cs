using System;
using System.Threading;
using System.Windows.Threading;

namespace Ann.Foundation
{
    public class RunOnTestDomain : MarshalByRefObject
    {
        private static int _testDomainCounter;

        public static void Do(CrossAppDomainDelegate action)
        {
            var i = Interlocked.Increment(ref _testDomainCounter);

            var testDomain = AppDomain.CreateDomain(
                "TestDomain" + i,
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);

            testDomain.DoCallBack(action.Invoke);

            testDomain.DoCallBack(() =>
            {
                // for appveyor 
                Dispatcher.CurrentDispatcher.InvokeShutdown();

                GC.Collect();
            });

            AppDomain.Unload(testDomain);
        }
    }
}