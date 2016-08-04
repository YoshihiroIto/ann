using System.Reflection;

namespace Ann.Foundation
{
    public class AssemblyConstants
    {
        public const string Company = "Jewelry Development";
        public const string Product = "Ann";

        public static string Version =>
            Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}