﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Ann.Core.Properties;
using Ann.Foundation;
using YamlDotNet.Serialization;
using System.Text;

namespace Ann.Core
{
    public static class Constants
    {
        public const double IconSize = 48;

        public static string AnnGitHubUrl = "https://github.com/YoshihiroIto/Ann";
        public static string AnnTwitterUrl = "https://twitter.com/yoiyoi322";

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

                var list = new List<OpenSource>();
                {
                    using (var reader = new StringReader(Encoding.UTF8.GetString(Resources.OpenSourceList)))
                        list.AddRange(new Deserializer().Deserialize<OpenSource[]>(reader));

                    using (var reader = new StringReader(Encoding.UTF8.GetString(Resources.OpenSourceListNonNuget)))
                        list.AddRange(new Deserializer().Deserialize<OpenSource[]>(reader));
                }

                _OpenSources = list.OrderBy(x => x.Name).ToArray();

                return _OpenSources;
            }
        }

        public static readonly CultureSummry[] SupportedCultures;

        public static string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return System.IO.Path.Combine(dir, AssemblyConstants.Company, AssemblyConstants.Product);
            }
        }

        static Constants()
        {
            var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? string.Empty;
            var resFiles = Directory.EnumerateFiles(dir, "Ann.resources.dll", SearchOption.AllDirectories);

            SupportedCultures = resFiles.Select(f =>
            {
                var name = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f));
                Debug.Assert(name != null);

                return new CultureSummry
                {
                    Caption = CultureInfo.GetCultureInfo(name).NativeName,
                    CultureName = name
                };
            })
                .Where(x => x != null)
                .ToArray();
        }
    }

    public enum IndexOpeningResults
    {
        InOpening,
        Ok,
        NotFound,
        OldIndex,
        CanNotOpen
    }
}