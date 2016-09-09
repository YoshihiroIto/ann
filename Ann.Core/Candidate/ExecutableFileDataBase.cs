using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using File = System.IO.File;

namespace Ann.Core.Candidate
{
    public class ExecutableFileDataBase : NotificationObject
    {
        private readonly App _app;
        private readonly string _indexFile;

        public ExecutableFileDataBase(App app, string indexFile)
        {
            Debug.Assert(app != null);

            _app = app;
            _indexFile = indexFile;
        }

        private ExecutableFile[] _ExecutableFiles;
        private IEnumerable<ExecutableFile> _prevResult;
        private string _prevKeyword;

        private bool IsOpend => _ExecutableFiles != null;

        public int ExecutableFileCount => IsOpend ? _ExecutableFiles.Length : 0;

        #region CrawlingCount

        private int _CrawlingCount;

        public int CrawlingCount
        {
            get { return _CrawlingCount; }
            set { SetProperty(ref _CrawlingCount, value); }
        }

        #endregion

        #region IndexOpeningProgress

        private int _IndexOpeningProgress;

        public int IndexOpeningProgress
        {
            get { return _IndexOpeningProgress; }
            set { SetProperty(ref _IndexOpeningProgress, value); }
        }

        #endregion

        private const int CurrentIndexVersion = 10;

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

            var targets = _prevKeyword == null || input.StartsWith(_prevKeyword, StringComparison.OrdinalIgnoreCase) == false
                ? _ExecutableFiles
                : _prevResult;

            var executableFileExtsArray = NormalizeExecutableFileExts(executableFileExts);

            using (new TimeMeasure($"Filtering -- {input}"))
            {
                var extScores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i != executableFileExtsArray.Length; ++i)
                    extScores[executableFileExtsArray[i]] = i;

                var inputs = input.Split(' ');
                var temp = new List<ExecutableFile> {Capacity = targets.Count()};

                var lockObj = new object();

                Parallel.ForEach(
                    targets,
                    () => new List<ExecutableFile> {Capacity = targets.Count()},
                    (u, loop, local) =>
                    {
                        foreach (var i in inputs)
                            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(u.SearchKey, i, CompareOptions.OrdinalIgnoreCase) == -1)
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
                _prevResult = temp;
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
            var ext = System.IO.Path.GetExtension(u.Path) ?? string.Empty;

            Debug.Assert(extScores.ContainsKey(ext));
            var extScore = extScores[ext];

            const int maxPathLength = 256;
            var pathLength = Math.Min(u.Path.Length, maxPathLength);

            {
                var score = MakeScoreSub(u.FileName, u.FileNameParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*0)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            {
                var score = MakeScoreSub(u.Name, u.NameParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*1)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            {
                // ReSharper disable once RedundantAssignment
                var score = MakeScoreSub(u.Directory, u.DirectoryParts, input);
                if (score != int.MaxValue)
                    return ((score + 4*2)*extScores.Count + extScore)*maxPathLength + pathLength;
            }

            return int.MaxValue;
        }

        private static int MakeScoreSub(string target, string[] targetParts, string input)
        {
            if (string.Equals(target, input, StringComparison.OrdinalIgnoreCase))
                return 0;

            if (target.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                return 1;

            if (targetParts != null)
                foreach (var t in targetParts)
                    if (t.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                        return 2;

            if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(target, input, CompareOptions.OrdinalIgnoreCase) == -1)
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

                using (new TimeMeasure("Index Serializing"))
                {
                    var ser = new Wire.Serializer();

                    using (var stream = new FileStream(_indexFile, FileMode.Create))
                    {
                        ser.Serialize(CurrentIndexVersion, stream);
                        ser.Serialize(_ExecutableFiles.Select(x => x.Path).ToArray(), stream);
                    }
                }
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
                        var ser = new Wire.Serializer();

                        using (var stream = new FileStream(_indexFile, FileMode.Open))
                        {
                            var version = (int) ser.Deserialize(stream);

                            if (version != CurrentIndexVersion)
                                return IndexOpeningResults.OldIndex;

                            var paths = (string[]) ser.Deserialize(stream);

                            var fileCount = paths.Length;
                            var tempExecutableFiles = new ExecutableFile[fileCount];
                            var isContainsInvalid = false;
                            var stringPool = new ConcurrentDictionary<string, string>();

                            var count = 0;

                            Parallel.For(
                                0,
                                fileCount,
                                i =>
                                {
                                    if (File.Exists(paths[i]) == false)
                                        isContainsInvalid = true;

                                    try
                                    {
                                        IndexOpeningProgress = 100*Interlocked.Increment(ref count)/fileCount;

                                        tempExecutableFiles[i] = new ExecutableFile(i, fileCount, paths[i],
                                            _app, _iconDecoder, stringPool, targetFoldersArray);
                                    }
                                    catch
                                    {
                                        isContainsInvalid = true;
                                    }
                                });

                            _ExecutableFiles =
                                isContainsInvalid
                                    ? tempExecutableFiles.Where(t => t != null).ToArray()
                                    : tempExecutableFiles;
                        }
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
            var executableExts = new HashSet<string>(executableFileExts, StringComparer.OrdinalIgnoreCase);

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
                                    .Where(f => executableExts.Contains(System.IO.Path.GetExtension(f)))
                                    .Select(f =>
                                    {
                                        CrawlingCount = Interlocked.Increment(ref count);
                                        return new ExecutableFile(f, _app, _iconDecoder, stringPool, targetFoldersArray);
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
                .Select(e => e[0] == '.' ? e : "." + e)
                .ToArray();
        }

        public ImageBrush GetIcon(string path) => _iconDecoder.GetIcon(path);
    }
}