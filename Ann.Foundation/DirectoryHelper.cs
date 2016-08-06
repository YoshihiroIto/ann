using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ann.Foundation
{
    public static class DirectoryHelper
    {
        public static IEnumerable<string> EnumerateAllFiles(string path)
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