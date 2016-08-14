using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace Ann.Foundation
{
    public static class TestHelper
    {
        public static void SetEntryAssembly()
        {
            // http://dejanstojanovic.net/aspnet/2015/january/set-entry-assembly-in-unit-testing-methods/

            var cd = Directory.GetCurrentDirectory();

#if DEBUG
            var mainAsmPath = Path.Combine(cd, @"..\..\..\Ann\bin\Debug\Ann.exe");
#else
            var mainAsmPath = Path.Combine(cd, @"..\..\..\Ann\bin\Release\Ann.exe");
#endif

            var assembly = Assembly.LoadFrom(mainAsmPath);
  
            var manager = new AppDomainManager();  
            var entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);  
            // ReSharper disable once PossibleNullReferenceException
            entryAssemblyfield.SetValue(manager, assembly);  
  
            var domain = AppDomain.CurrentDomain;  
            var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);  
            // ReSharper disable once PossibleNullReferenceException
            domainManagerField.SetValue(domain, manager);
        }
    }

    public class RunOnTestDomain : MarshalByRefObject
    {
        private static int _testDomainCounter;

        public static void Do(CrossAppDomainDelegate action)
        {
            var i = Interlocked.Increment(ref _testDomainCounter);

            var testDomain = AppDomain.CreateDomain("TestDomain" + i, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

            testDomain.DoCallBack(action);
            testDomain.DoCallBack(() =>
            {
                // for appveyor 
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            });

            AppDomain.Unload(testDomain);
        }
    }
}