using System;
using System.Diagnostics;

namespace Ann.Core
{
    [DebuggerDisplay("Id:{_id}, MaxId:{_maxId}, Rank:{_rank}, Path:{Path}")]
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
        private int _id;
        private int _maxId;
        private int _rank;

        public void SetRank(int r)
        {
            if (r == int.MaxValue)
                _rank = int.MaxValue;
            else
                _rank = r*_maxId + _id;
        }

        public void SetId(int id, int maxId)
        {
            _id = id;
            _maxId = maxId;
        }

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

        public ExecutableUnit(int id, int maxId, string path)
            : this(path)
        {
            SetId(id, maxId);
        }

        private static readonly char[] Separator = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableUnit>.CompareTo(ExecutableUnit other) => _rank - other._rank;
    }
}