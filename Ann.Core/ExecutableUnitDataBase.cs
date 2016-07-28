using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ann.Foundation;
using FlatBuffers;
using IndexFile;
using File = System.IO.File;

namespace Ann.Core
{
    public class ExecutableUnitDataBase
    {
        private readonly string _indexFile;

        public ExecutableUnitDataBase(string indexFile)
        {
            _indexFile = indexFile;
        }

        private ExecutableUnit[] _executableUnits;
        private ExecutableUnit[] _prevResult;
        private string _prevKeyword;

        private bool IsOpend => _executableUnits != null;

        private const Versions CurrentIndexVersion = Versions.V1;

        public IEnumerable<ExecutableUnit> Find(string keyword, IEnumerable<string> executableFileExts)
        {
            if (keyword == null)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableUnit>();
            }

            if (IsOpend == false)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableUnit>();
            }

            keyword = keyword.Trim();

            if (keyword == string.Empty)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableUnit>();
            }
            keyword = keyword.ToLower();

            var targets = _prevKeyword == null || keyword.StartsWith(_prevKeyword) == false
                ? _executableUnits
                : _prevResult;

            using (new TimeMeasure("Filtering"))
            {
                var extRanks = new Dictionary<string, int>();
                executableFileExts.ForEach((e, i) => extRanks["." + e] = i);

                var keywords = keyword.Split(' ');

#if false
                _prevResult = targets
                    .AsParallel()
                    .Where(u => keywords.All(u.SearchKey.Contains))
                    .OrderBy(u => keywords.Sum(k => MakeRank(u, k, extRanks))/keywords.Length)
                    .ToArray();
#else
                _prevResult = targets
                    .AsParallel()
                    .Where(u =>
                    {
                        if (keywords.All(u.SearchKey.Contains) == false)
                            return false;

                        u.Rank = keywords.Sum(k => MakeRank(u, k, extRanks))/keywords.Length;
                        return true;
                    })
                    .OrderBy(u => u.Rank)
                    .ToArray();
#endif
            }

            _prevKeyword = keyword;

            return _prevResult;
        }

        public async Task<IndexOpeningResults> UpdateIndexAsync(IEnumerable<string> targetFolders,
            IEnumerable<string> executableFileExts)
        {
            using (new TimeMeasure("Index Crawlering"))
                _executableUnits = await CrawlAsync(targetFolders, executableFileExts);

            if (_executableUnits == null)
                return IndexOpeningResults.CanNotOpen;

            await Task.Run(() =>
            {
                var dir = System.IO.Path.GetDirectoryName(_indexFile);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                byte[] data;

                using (new TimeMeasure("Index Serializing"))
                {
                    var fbb = new FlatBufferBuilder(1);

                    var euOffsets = new Offset<IndexFile.ExecutableUnit>[_executableUnits.Length];

                    for (var i = 0; i != _executableUnits.Length; ++ i)
                    {
                        euOffsets[i] =
                            IndexFile.ExecutableUnit.CreateExecutableUnit(fbb,
                                fbb.CreateString(_executableUnits[i].Path),
                                fbb.CreateString(_executableUnits[i].Name),
                                fbb.CreateString(_executableUnits[i].LowerName),
                                fbb.CreateString(_executableUnits[i].LowerDirectory),
                                fbb.CreateString(_executableUnits[i].LowerFileName),
                                fbb.CreateString(_executableUnits[i].SearchKey));
                    }

                    var rowsOffset = IndexFile.File.CreateRowsVector(fbb, euOffsets);

                    IndexFile.File.StartFile(fbb);
                    IndexFile.File.AddVersion(fbb, CurrentIndexVersion);
                    IndexFile.File.AddRows(fbb, rowsOffset);

                    var endFile = IndexFile.File.EndFile(fbb);

                    fbb.Finish(endFile.Value);

                    data = fbb.SizedByteArray();
                }

                File.WriteAllBytes(_indexFile, data);
            });

            return IndexOpeningResults.Ok;
        }

        public IndexOpeningResults OpenIndex()
        {
            _executableUnits = null;

            if (File.Exists(_indexFile) == false)
                return IndexOpeningResults.NotFound;

            try
            {
                var data = new ByteBuffer(File.ReadAllBytes(_indexFile));

                using (new TimeMeasure("Index Deserializing"))
                {
                    var root = IndexFile.File.GetRootAsFile(data);

                    if (root.Version != CurrentIndexVersion)
                        return IndexOpeningResults.OldIndex;

                    _executableUnits = new ExecutableUnit[root.RowsLength];

                    var temp = new IndexFile.ExecutableUnit();

                    for (var i = 0; i != root.RowsLength; ++i)
                    {
                        root.GetRows(temp, i);

                        _executableUnits[i] = new ExecutableUnit
                        {
                            Path = temp.Path,
                            Name = temp.Name,
                            LowerName = temp.LowerName,
                            LowerDirectory = temp.LowerDirectory,
                            LowerFileName = temp.LowerFileName,
                            SearchKey = temp.SearchKey
                        };
                    }
                }

                return IndexOpeningResults.Ok;
            }
            catch
            {
                return IndexOpeningResults.CanNotOpen;
            }
        }

        // ReSharper disable PossibleNullReferenceException
        private static int MakeRank(ExecutableUnit u, string name, Dictionary<string, int> extRanks)
        {
            Func<int, string, int> makeRankSub = (rankBase, target) =>
            {
                if (target == name)
                    return (rankBase + 0)*2;

                var parts = target.Split(new[] {' ', '_', '-', '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Any(p => p.StartsWith(name)))
                    return (rankBase + 1)*2;

                return int.MaxValue;
            };

            var ext = System.IO.Path.GetExtension(u.Path).ToLower();

            Debug.Assert(extRanks.ContainsKey(ext));
            var extRank = extRanks[ext];

            var b = 0;

            var rankFileName = makeRankSub(++b, u.LowerFileName);
            if (rankFileName != int.MaxValue)
                return (rankFileName * extRanks.Count + extRank) * 200 + u.LowerFileName.Length;
 
            var rankName = makeRankSub(++b, u.LowerName);
            if (rankName != int.MaxValue)
                return (rankName * extRanks.Count + extRank) * 200 + u.LowerName.Length;

            var rankDir = makeRankSub(++b, u.LowerDirectory);
            if (rankDir != int.MaxValue)
                return (rankDir * extRanks.Count + extRank)  * 200 + u.LowerDirectory.Length;

            return int.MaxValue;
        }
        // ReSharper restore PossibleNullReferenceException

#region Crawler

        private static async Task<ExecutableUnit[]> CrawlAsync(
            IEnumerable<string> targetFolders,
            IEnumerable<string> executableFileExts)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var executableExts = new HashSet<string>(executableFileExts.Select(e => "." + e.ToLower()));

                    return targetFolders
                        .AsParallel()
                        .SelectMany(targetFolder =>
                            EnumerateAllFiles(targetFolder)
                                .Where(f => executableExts.Contains(System.IO.Path.GetExtension(f)?.ToLower()))
                                .Select(f =>
                                {
                                    var fvi = FileVersionInfo.GetVersionInfo(f);

                                    var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                                        ? System.IO.Path.GetFileNameWithoutExtension(f)
                                        : fvi.FileDescription;

                                    var eu = new ExecutableUnit
                                    {
                                        Path = f,
                                        Name = name,
                                        LowerName = name.ToLower(),
                                        LowerDirectory = (System.IO.Path.GetDirectoryName(f) ?? string.Empty).ToLower(),
                                        LowerFileName = System.IO.Path.GetFileNameWithoutExtension(f).ToLower()
                                    };

                                    eu.SearchKey = $"{eu.LowerName}*{eu.LowerDirectory}*{eu.LowerFileName}";

                                    return eu;
                                })
                        ).ToArray();
                }
                catch
                {
                    return null;
                }
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

#endregion
    }
}