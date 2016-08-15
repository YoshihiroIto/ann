using System.IO;
using System.Reflection;
using System.Threading;

namespace Ann.Foundation
{
    public class AssemblyConstants
    {
        public const string Company = "Jewelry Development";
        public const string Product = "Ann";
        public const string Copyright = "Copyright © 2016 Jewelry Development All Rights Reserved.";

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static string _EntryAssemblyLocation;
        public static string EntryAssemblyLocation
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _EntryAssemblyLocation, () =>
                {
                    var e = Assembly.GetEntryAssembly();
                    if (e != null)
                        return e.Location;

                    var cd = Directory.GetCurrentDirectory();
#if DEBUG
                    var mainAsmPath = Path.Combine(cd, @"..\..\..\Ann\bin\Debug\Ann.exe");
#else
                    var mainAsmPath = Path.Combine(cd, @"..\..\..\Ann\bin\Release\Ann.exe");
#endif
                    return mainAsmPath;
                });
            }
        }

        private static Assembly _EntryAssembly;
        public static Assembly EntryAssembly
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _EntryAssembly,
                    () => Assembly.GetEntryAssembly() ?? Assembly.LoadFrom(EntryAssemblyLocation));
            }
        }
    }
}