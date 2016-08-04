using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public ExecutableUnit(
            string path,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
        {
            var fvi = FileVersionInfo.GetVersionInfo(path);

            var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                ? System.IO.Path.GetFileNameWithoutExtension(path)
                : fvi.FileDescription;

            Path = stringPool.GetOrAdd(path, path);
            Name = stringPool.GetOrAdd(name, name);
            LowerName = stringPool.GetOrAdd(name.ToLower(), s => s);

            var dir = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
            dir = ShrinkDir(dir, targetFolders);

            LowerDirectory = stringPool.GetOrAdd(dir, s => s);
            LowerFileName = stringPool.GetOrAdd(System.IO.Path.GetFileNameWithoutExtension(path).ToLower(), s => s);
            SearchKey = stringPool.GetOrAdd($"{LowerName}*{LowerDirectory}*{LowerFileName}", s => s);

            LowerNameParts = LowerName.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            LowerDirectoryParts = LowerDirectory.Split(new [] { '\\'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            LowerFileNameParts = LowerFileName.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            if (LowerNameParts.Length <= 1)
                LowerNameParts = null;

            if (LowerDirectoryParts.Length <= 1)
                LowerDirectoryParts = null;

            if (LowerFileNameParts.Length <= 1)
                LowerFileNameParts = null;
        }

        public ExecutableUnit(
            int id, int maxId, string path,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
            : this(path, stringPool, targetFolders)
        {
            SetId(id, maxId);
        }

        private static string ShrinkDir(string srcDir, IEnumerable<string> targetFolders)
        {
            var srcLower = srcDir.ToLower();

            foreach (var f in targetFolders)
                if (srcLower.StartsWith(f))
                    return srcLower.Substring(f.Length);

            return srcDir;
        }

        private static readonly char[] Separator = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableUnit>.CompareTo(ExecutableUnit other) => _rank - other._rank;
    }
}