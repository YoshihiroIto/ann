using System.Reflection;

namespace Ann.Foundation
{
    public class AssemblyConstants
    {
        public const string Company = "Jewelry Development";
        public const string Product = "Ann";
        public const string Copyright = "Copyright © 2016 Jewelry Development All Rights Reserved.";

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}