using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ann.Foundation;
using Jil;

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

            var dir = Path.GetDirectoryName(_indexFile);
            if (dir != null)
                Directory.CreateDirectory(dir);

            string text;

            using (new TimeMeasure("Index Serializing"))
                text = JSON.Serialize(_executableUnits);

            File.WriteAllText(_indexFile, text);

            Opend?.Invoke(this, EventArgs.Empty);
        }

        public async Task OpenAsync()
        {
            await Task.Run(() =>
            {
                if (File.Exists(_indexFile) == false)
                    return;

                var text = File.ReadAllText(_indexFile);

                using (new TimeMeasure("Index Deserializing"))
                    _executableUnits = JSON.Deserialize<ExecutableUnit[]>(text);

                Opend?.Invoke(this, EventArgs.Empty);
            });
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