﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ann.Core
{
    public static class Crawler
    {
        public static async Task<ExecutableUnit[]> ExecuteAsync(IEnumerable<string> targetFolders)
        {
            return await Task.Run(() =>
            {
                var executableExts = new HashSet<string> {".exe", ".lnk"};

                return targetFolders
                    .SelectMany(targetFolder =>
                        EnumerateAllFiles(targetFolder)
                            .Where(f => executableExts.Contains(Path.GetExtension(f)?.ToLower()))
                            .Select(f =>
                            {
                                var fvi = FileVersionInfo.GetVersionInfo(f);

                                var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                                    ? Path.GetFileNameWithoutExtension(f)
                                    : fvi.FileDescription;

                                return new ExecutableUnit
                                {
                                    Path = f,
                                    Name = name,
                                    LowerName = name.ToLower(),
                                    LowerDirectory = (Path.GetDirectoryName(f) ?? string.Empty).ToLower(),
                                    LowerFileName = Path.GetFileNameWithoutExtension(f).ToLower(),
                                    SearchKey = $"{name}*{Path.GetDirectoryName(f) ?? string.Empty}*{Path.GetFileNameWithoutExtension(f)}"
                                };
                            })
                    ).ToArray();
            });
        }

        private static IEnumerable<string> EnumerateAllFiles(string path)
        {
            try
            {
                var dirFiles = Directory.EnumerateDirectories(path)
                    .SelectMany(EnumerateAllFiles);

                return dirFiles.Concat(Directory.EnumerateFiles(path));
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}