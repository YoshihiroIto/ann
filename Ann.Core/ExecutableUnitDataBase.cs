using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ann.Foundation;
using FlatBuffers;

namespace Ann.Core
{
    public class ExecutableUnitDataBase : IDisposable
    {
        public event EventHandler Opend;
        public event EventHandler Closed;

        private bool _isOpend;

        private readonly string _indexFile;

        public ExecutableUnitDataBase(string indexFile)
        {
            _indexFile = indexFile;
            Opend += (_, __) => _isOpend = true;
            Closed += (_, __) => _isOpend = false;
        }

        public void Dispose()
        {
            Close();
        }

        private string _prevKeyword;
        private ExecutableUnit[] _prevResult;

        public IEnumerable<ExecutableUnit> Find(string keyword)
        {
            if (keyword == null)
            {
                _prevKeyword = null;
                _prevResult = null;
                return Enumerable.Empty<ExecutableUnit>();
            }

            if (_isOpend == false)
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

            var target = _prevKeyword == null || keyword.StartsWith(_prevKeyword) == false
                ? _executableUnits
                : _prevResult;

            using (new TimeMeasure("Filtering"))
            {
                _prevResult = target
                    .AsParallel()
                    .Where(u => u.SearchKey.Contains(keyword))
                    .OrderBy(u => MakeRank(u, keyword))
                    .ToArray();
            }

            _prevKeyword = keyword;

            return _prevResult;
        }

        private ExecutableUnit[] _executableUnits;

        public async Task UpdateIndexAsync(IEnumerable<string> targetFolders)
        {
            using (new TimeMeasure("Index Crawlering"))
                _executableUnits = await Crawler.ExecuteAsync(targetFolders);

            await Task.Run(() =>
            {
                var dir = Path.GetDirectoryName(_indexFile);
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
                    IndexFile.File.AddRows(fbb, rowsOffset);

                    var endFile = IndexFile.File.EndFile(fbb);

                    fbb.Finish(endFile.Value);

                    data = fbb.SizedByteArray();
                }

                File.WriteAllBytes(_indexFile, data);

                Opend?.Invoke(this, EventArgs.Empty);
            });
        }

        public void Open()
        {
            if (File.Exists(_indexFile) == false)
                return;

            var data = new ByteBuffer(File.ReadAllBytes(_indexFile));

            using (new TimeMeasure("Index Deserializing"))
            {
                var root = IndexFile.File.GetRootAsFile(data);

                _executableUnits = new ExecutableUnit[root.RowsLength];

                var temp = new IndexFile.ExecutableUnit();

                for (var i = 0; i != root.RowsLength; ++i)
                {
                    root.GetRows(temp, i);

                    _executableUnits[i].Path = temp.Path;
                    _executableUnits[i].Name = temp.Name;
                    _executableUnits[i].LowerName = temp.LowerName;
                    _executableUnits[i].LowerDirectory = temp.LowerDirectory;
                    _executableUnits[i].LowerFileName = temp.LowerFileName;
                    _executableUnits[i].SearchKey = temp.SearchKey;
                }
            }

            Opend?.Invoke(this, EventArgs.Empty);
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        // ReSharper disable PossibleNullReferenceException
        private static int MakeRank(ExecutableUnit u, string name)
        {
            var unitNameLength = Math.Min(u.Name.Length, 9999);

            Func<int, string, int> makeRankSub = (rankBase, target) =>
            {
                if (target == name)
                    return (rankBase + 0)*10000 + unitNameLength;

                var parts = target.Split(new[] {' ', '_', '-', '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Any(t => t.StartsWith(name)))
                    return (rankBase + 1)*10000 + unitNameLength;

                if (target.StartsWith(name))
                    return (rankBase + 2)*10000 + unitNameLength;

                if (target.Contains(name))
                    return (rankBase + 3)*10000 + unitNameLength;

                return int.MaxValue;
            };

            var b = 0;

            var rankFileName = makeRankSub(++b, u.LowerFileName);
            if (rankFileName != int.MaxValue)
                return rankFileName;

            var rankName = makeRankSub(++b, u.LowerName);
            if (rankName != int.MaxValue)
                return rankName;

            var rankDir = makeRankSub(++b, u.LowerDirectory);
            if (rankDir != int.MaxValue)
                return rankDir;

            return int.MaxValue;
        }

        // ReSharper restore PossibleNullReferenceException
    }
}