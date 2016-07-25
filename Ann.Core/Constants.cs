using System;
using System.IO;
using System.Reflection;
using Ann.Core.Properties;
using Ann.Foundation;
using YamlDotNet.Serialization;

namespace Ann.Core
{
    public static class Constants
    {
        public const double IconSize = 48;

        public static string SystemFolder => Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static string SystemX86Folder => Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static string ProgramsFolder => Environment.GetFolderPath(Environment.SpecialFolder.Programs);

        public static string ProgramFilesFolder =>
            Environment.Is64BitProcess
                ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                : Environment.GetEnvironmentVariable("ProgramW6432");

        public static string ProgramFilesX86Folder =>
            Environment.Is64BitProcess
                ? Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        private static OpenSource[] _OpenSources;

        public static OpenSource[] OpenSources
        {
            get
            {
                if (_OpenSources != null)
                    return _OpenSources;

                try
                {
                    var yaml = System.Text.Encoding.UTF8.GetString(Resources.OpenSourceList);
                    using (var reader = new StringReader(yaml))
                        _OpenSources = new Deserializer().Deserialize<OpenSource[]>(reader);

                    return _OpenSources;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string CompanyName =>
            ((AssemblyCompanyAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false))
                .Company;

        public static string ProductName =>
            ((AssemblyProductAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute), false))
                .Product;
    }

    public enum IndexOpeningResults
    {
        Ok,
        NotFound,
        OldIndex,
        CanNotOpen
    }
}