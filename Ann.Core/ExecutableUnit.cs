using System;
using System.Diagnostics;

namespace Ann.Core
{
    public class ExecutableUnit : IComparable<ExecutableUnit>
    {
        public readonly string Path;
        public readonly string Name;
        public readonly string LowerName;
        public readonly string LowerDirectory;
        public readonly string LowerFileName;
        public readonly string SearchKey;

        public readonly string[] LowerNameParts;
        public readonly string[] LowerDirectoryParts;
        public readonly string[] LowerFileNameParts;

        //
        public int Rank;

        public ExecutableUnit(string path)
        {
            var fvi = FileVersionInfo.GetVersionInfo(path);

            var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                ? System.IO.Path.GetFileNameWithoutExtension(path)
                : fvi.FileDescription;

            Path = path;
            Name = name;
            LowerName = name.ToLower();
            LowerDirectory = (System.IO.Path.GetDirectoryName(path) ?? string.Empty).ToLower();
            LowerFileName = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();
            SearchKey = $"{LowerName}*{LowerDirectory}*{LowerFileName}";

            LowerNameParts = LowerName.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            LowerDirectoryParts = LowerDirectory.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            LowerFileNameParts = LowerFileName.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private static readonly char[] Separator = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableUnit>.CompareTo(ExecutableUnit other) => Rank - other.Rank;
    }
}