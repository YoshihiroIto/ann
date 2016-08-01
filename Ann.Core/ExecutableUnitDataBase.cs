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

        private const Versions CurrentIndexVersion = Versions.Version;

        public IEnumerable<ExecutableUnit> Find(string input, IEnumerable<string> executableFileExts)
        {
            if (input == null)
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

            input = input.Trim();

            if (input == string.Empty)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableUnit>();
            }

            input = input.ToLower();

            var targets = _prevKeyword == null || input.StartsWith(_prevKeyword) == false
                ? _executableUnits
                : _prevResult;

            using (new TimeMeasure("Filtering"))
            {
                var extRanks = new Dictionary<string, int>();
                executableFileExts.ForEach((e, i) => extRanks["." + e] = i);

                var inputs = input.Split(' ');
                var temp = new List<ExecutableUnit> {Capacity = targets.Length};

                var lockObj = new object();

                Parallel.ForEach(
                    targets,
                    () => new List<ExecutableUnit> {Capacity = targets.Length},
                    (u, loop, local) =>
                    {
                        if (inputs.All(u.SearchKey.Contains) == false)
                            return local;

                        u.SetRank(MakeRank(u, inputs, extRanks));
                        local.Add(u);

                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            temp.AddRange(local);
                        }
                    });

                temp.Sort();
                _prevResult = temp.ToArray();
            }

            _prevKeyword = input;

            return _prevResult;
        }

        private static int MakeRank(ExecutableUnit u, string[] inputs, Dictionary<string, int> extRanks)
        {
            var rank = 0;

            foreach (var i in inputs)
            {
                var r = MakeRank(u, i, extRanks);
                if (r == int.MaxValue)
                    return int.MaxValue;

                rank += r;
            }

            return rank/inputs.Length;
        }

        private static int MakeRank(ExecutableUnit u, string input, Dictionary<string, int> extRanks)
        {
            // ReSharper disable once PossibleNullReferenceException
            var ext = System.IO.Path.GetExtension(u.Path).ToLower();

            Debug.Assert(extRanks.ContainsKey(ext));
            var extRank = extRanks[ext];

            var b = 0;

            var rankFileName = MakeRankSub(++b, u.LowerFileName, u.LowerFileNameParts, input);
            if (rankFileName != int.MaxValue)
                return (rankFileName*extRanks.Count + extRank)*200 + u.LowerFileName.Length;

            var rankName = MakeRankSub(++b, u.LowerName, u.LowerNameParts, input);
            if (rankName != int.MaxValue)
                return (rankName*extRanks.Count + extRank)*200 + u.LowerName.Length;

            var rankDir = MakeRankSub(++b, u.LowerDirectory, u.LowerDirectoryParts, input);
            if (rankDir != int.MaxValue)
                return (rankDir*extRanks.Count + extRank)*200 + u.LowerDirectory.Length;

            return int.MaxValue;
        }

        private static int MakeRankSub(int rankBase, string target, string[] targetParts, string input)
        {
            if (target == input)
                return (rankBase + 0)*2;

            if (targetParts.Any(p => p.StartsWith(input)))
                return (rankBase + 1)*2;

            return int.MaxValue;
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
                            IndexFile.ExecutableUnit.CreateExecutableUnit(
                                fbb,
                                i,
                                fbb.CreateString(_executableUnits[i].Path));
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

        public async Task<IndexOpeningResults> OpenIndexAsync()
        {
            return await Task.Run(() =>
            {
                if (File.Exists(_indexFile) == false)
                    return IndexOpeningResults.NotFound;

                try
                {
                    using (new TimeMeasure("Index Deserializing"))
                    {
                        var data = new ByteBuffer(File.ReadAllBytes(_indexFile));
                        var root = IndexFile.File.GetRootAsFile(data);

                        if (root.Version != CurrentIndexVersion)
                            return IndexOpeningResults.OldIndex;

                        var tempExecutableUnits = new ExecutableUnit[root.RowsLength];
                        var isContainsInvalid = false;

                        Parallel.For(
                            0,
                            root.RowsLength,
                            () => new IndexFile.ExecutableUnit(),
                            (i, loop, rowTemp) =>
                            {
                                root.GetRows(rowTemp, i);

                                try
                                {
                                    tempExecutableUnits[i] = new ExecutableUnit(i, root.RowsLength, rowTemp.Path);
                                }
                                catch
                                {
                                    tempExecutableUnits[i] = null;
                                    isContainsInvalid = true;
                                }

                                return rowTemp;
                            },
                            rowTemp => { });

                        _executableUnits =
                            isContainsInvalid
                                ? tempExecutableUnits.Where(t => t != null).ToArray()
                                : tempExecutableUnits;
                    }

                    return IndexOpeningResults.Ok;
                }
                catch
                {
                    _executableUnits = null;
                    return IndexOpeningResults.CanNotOpen;
                }
            });
        }

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

                    var results = targetFolders
                        .AsParallel()
                        .SelectMany(targetFolder =>
                            EnumerateAllFiles(targetFolder)
                                .Where(f => executableExts.Contains(System.IO.Path.GetExtension(f)?.ToLower()))
                                .Select(f => new ExecutableUnit(f))
                        ).ToArray();

                    results.ForEach((r, i) => r.SetId(i, results.Length));

                    return results;
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