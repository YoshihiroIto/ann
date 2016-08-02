using System;
using System.Reflection;

namespace Ann.Foundation
{
    public class AssemblyConstants
    {
        public static string CompanyName =>
            ((AssemblyCompanyAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false))
                .Company;

        public static string ProductName =>
            ((AssemblyProductAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute), false))
                .Product;

        public static string Version =>
            Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}