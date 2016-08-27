using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using FlatBuffers;
using IndexFile;
using File = System.IO.File;

namespace Ann.Core
{
    public class ExecutableFileDataBase : NotificationObject
    {
        private readonly string _indexFile;

        public ExecutableFileDataBase(string indexFile)
        {
            _indexFile = indexFile;
        }

        private ExecutableFile[] _ExecutableFiles;
        private ExecutableFile[] _prevResult;
        private string _prevKeyword;

        private bool IsOpend => _ExecutableFiles != null;

        private const Versions CurrentIndexVersion = Versions.Version;

        public int ExecutableFileCount => IsOpend ? _ExecutableFiles.Length : 0;

        #region CrawlingCount

        private int _CrawlingCount;

        public int CrawlingCount
        {
            get { return _CrawlingCount; }
            set { SetProperty(ref _CrawlingCount, value); }
        }

        #endregion

        public int IconCacheSize
        {
            get { return _iconDecoder.IconCacheSize; }
            set { _iconDecoder.IconCacheSize = value; }
        }

        private readonly IconDecoder _iconDecoder = new IconDecoder();

        public IEnumerable<ExecutableFile> Find(string input, IEnumerable<string> executableFileExts)
        {
            if (input == null)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableFile>();
            }

            if (IsOpend == false)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableFile>();
            }

            input = input.Trim();

            if (input == string.Empty)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableFile>();
            }

            input = input.ToLower();

            var targets = _prevKeyword == null || input.StartsWith(_prevKeyword) == false
                ? _ExecutableFiles
                : _prevResult;

            var executableFileExtsArray = NormalizeExecutableFileExts(executableFileExts);

            using (new TimeMeasure($"Filtering -- {input}"))
            {
                var extScores = new Dictionary<string, int>();
                executableFileExtsArray.ForEach((e, i) => extScores[e] = i);

                var inputs = input.Split(' ');
                var temp = new List<ExecutableFile> {Capacity = targets.Length};

                var lockObj = new object();

                Parallel.ForEach(
                    targets,
                    () => new List<ExecutableFile> {Capacity = targets.Length},
                    (u, loop, local) =>
                    {
                        if (inputs.All(u.SearchKey.Contains) == false)
                            return local;

                        u.SetScore(MakeScore(u, inputs, extScores));
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

        private static int MakeScore(ExecutableFile u, string[] inputs, Dictionary<string, int> extScores)
        {
            var score = 0;

            foreach (var i in inputs)
            {
                var r = MakeScore(u, i, extScores);
                if (r == int.MaxValue)
                    return int.MaxValue;

                score += r;
            }

            return score/inputs.Length;
        }

        private static int MakeScore(ExecutableFile u, string input, Dictionary<string, int> extScores)
        {
            // ReSharper disable once PossibleNullReferenceException
            var ext = System.IO.Path.GetExtension(u.Path).ToLower();

            Debug.Assert(extScores.ContainsKey(ext));
            var extScore = extScores[ext];

            const int maxPathLength = 256;
            var pathLength = Math.Min(u.Path.Length, maxPathLength);

            {
                var score = MakeScoreSub(u.LowerFileName, u.LowerFileNameParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*0)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            {
                var score = MakeScoreSub(u.LowerName, u.LowerNameParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*1)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            {
                // ReSharper disable once RedundantAssignment
                var score = MakeScoreSub(u.LowerDirectory, u.LowerDirectoryParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*2)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            return int.MaxValue;
        }

        private static int MakeScoreSub(string target, string[] targetParts, string input)
        {
            if (target == input)
                return 0;

            if (target.StartsWith(input))
                return 1;

            if (targetParts != null)
                if (targetParts.Any(p => p.StartsWith(input)))
                    return 2;

            if (target.Contains(input))
                return 3;

            return int.MaxValue;
        }

        public async Task CancelUpdateIndexAsync()
        {
            if (_crawlingTokenSource == null)
                return;

            await Task.Run(() =>
            {
                _crawlingTokenSource?.Cancel();
                _crawlingResetEvent?.Wait();
            });
        }

        public async Task<IndexOpeningResults> UpdateIndexAsync(
            IEnumerable<string> targetFolders,
            IEnumerable<string> executableFileExts)
        {
            var targetFoldersArray = NormalizeTargetFolders(targetFolders);
            var executableFileExtsArray = NormalizeExecutableFileExts(executableFileExts);

            using (new TimeMeasure("Index Crawlering"))
            {
                var result = await CrawlAsync(targetFoldersArray, executableFileExtsArray);
                if (result.IsCanceled)
                    return IndexOpeningResults.Ok;

                _ExecutableFiles = result.Files;
            }

            if (_ExecutableFiles == null)
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

                    var euOffsets = new Offset<IndexFile.ExecutableFile>[_ExecutableFiles.Length];

                    for (var i = 0; i != _ExecutableFiles.Length; ++i)
                    {
                        euOffsets[i] =
                            IndexFile.ExecutableFile.CreateExecutableFile(
                                fbb,
                                fbb.CreateString(_ExecutableFiles[i].Path));
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

        public async Task<IndexOpeningResults> OpenIndexAsync(IEnumerable<string> targetFolders)
        {
            var targetFoldersArray = NormalizeTargetFolders(targetFolders);

            if (File.Exists(_indexFile) == false)
                return IndexOpeningResults.NotFound;

            return await Task.Run(() =>
            {
                try
                {
                    using (new TimeMeasure("Index Deserializing"))
                    {
                        var data = new ByteBuffer(File.ReadAllBytes(_indexFile));
                        var root = IndexFile.File.GetRootAsFile(data);

                        if (root.Version != CurrentIndexVersion)
                            return IndexOpeningResults.OldIndex;

                        var tempExecutableFiles = new ExecutableFile[root.RowsLength];
                        var isContainsInvalid = false;
                        var stringPool = new ConcurrentDictionary<string, string>();

                        Parallel.For(
                            0,
                            root.RowsLength,
                            () => new IndexFile.ExecutableFile(),
                            (i, loop, rowTemp) =>
                            {
                                root.GetRows(rowTemp, i);

                                if (File.Exists(rowTemp.Path) == false)
                                {
                                    isContainsInvalid = true;
                                    return rowTemp;
                                }

                                try
                                {
                                    tempExecutableFiles[i] = new ExecutableFile(i, root.RowsLength, rowTemp.Path,
                                        _iconDecoder, stringPool, targetFoldersArray);
                                }
                                catch
                                {
                                    isContainsInvalid = true;
                                }

                                return rowTemp;
                            },
                            rowTemp => { });

                        _ExecutableFiles =
                            isContainsInvalid
                                ? tempExecutableFiles.Where(t => t != null).ToArray()
                                : tempExecutableFiles;
                    }

                    return IndexOpeningResults.Ok;
                }
                catch
                {
                    _ExecutableFiles = null;
                    return IndexOpeningResults.CanNotOpen;
                }
            });
        }

        private CancellationTokenSource _crawlingTokenSource;
        private ManualResetEventSlim _crawlingResetEvent;
        private readonly object _crawlingLock = new object();

        private class CrawlingResult
        {
            public ExecutableFile[] Files { get; set; }
            public bool IsCanceled { get; set; }
        }

        private async Task<CrawlingResult> CrawlAsync(
            string[] targetFolders,
            IEnumerable<string> executableFileExts)
        {
            var targetFoldersArray = NormalizeTargetFolders(targetFolders);
            var executableExts = new HashSet<string>(executableFileExts);

            return await Task.Run(() =>
            {
                try
                {
                    lock (_crawlingLock)
                    {
                        _crawlingTokenSource = new CancellationTokenSource();
                        _crawlingResetEvent = new ManualResetEventSlim();
                    }

                    var stringPool = new ConcurrentDictionary<string, string>();
                    var count = 0;

                    var results = targetFoldersArray
                        .AsParallel()
                        .WithCancellation(_crawlingTokenSource.Token)
                        .SelectMany(targetFolder =>
                                DirectoryHelper.EnumerateAllFiles(targetFolder)
                                    .Where(f => executableExts.Contains(System.IO.Path.GetExtension(f)?.ToLower()))
                                    .Select(f =>
                                    {
                                        CrawlingCount = Interlocked.Increment(ref count);
                                        return new ExecutableFile(f, _iconDecoder, stringPool, targetFoldersArray);
                                    })
                        ).ToArray();

                    results.ForEach((r, i) => r.SetId(i, results.Length));

                    return new CrawlingResult
                    {
                        Files = results,
                        IsCanceled = false,
                    };
                }
                catch (OperationCanceledException)
                {
                    return new CrawlingResult
                    {
                        IsCanceled = true
                    };
                }
                catch
                {
                    return new CrawlingResult
                    {
                        IsCanceled = false
                    };
                }
                finally
                {
                    lock (_crawlingLock)
                    {
                        _crawlingTokenSource.Dispose();
                        _crawlingTokenSource = null;

                        _crawlingResetEvent.Set();
                        _crawlingResetEvent.Dispose();
                        _crawlingResetEvent = null;
                    }
                }
            });
        }

        private static string[] NormalizeTargetFolders(IEnumerable<string> targetFolders)
        {
            return targetFolders.Select(Environment.ExpandEnvironmentVariables)
                .Distinct()
                .Where(Directory.Exists)
                .Select(f =>
                {
                    f = f.Replace('/', '\\');
                    f = f.TrimEnd('\\') + '\\';
                    return f;
                })
                .OrderByDescending(f => f.Length)
                .ToArray();
        }

        private static string[] NormalizeExecutableFileExts(IEnumerable<string> executableFileExts)
        {
            return executableFileExts
                .Select(e => e[0] == '.' ? e.ToLower() : "." + e.ToLower())
                .ToArray();
        }

        public ImageSource GetIcon(string path) => _iconDecoder.GetIcon(path);
    }
}