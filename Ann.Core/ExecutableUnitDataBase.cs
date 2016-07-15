using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ann.Core
{
    public class ExecutableUnitDataBase : IDisposable
    {
        private SQLiteConnection _conn;

        private readonly string _dataBaseFile;

        public ExecutableUnitDataBase(string databaseFile)
        {
            _dataBaseFile = databaseFile;

            if (File.Exists(databaseFile) == false)
                return;

            Open();
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<ExecutableUnit> Find(string name)
        {
            if (name == null)
                return Enumerable.Empty<ExecutableUnit>();

            name = name.Trim();

            if (name == string.Empty)
                return Enumerable.Empty<ExecutableUnit>();

            if (_conn == null)
                return Enumerable.Empty<ExecutableUnit>();

            using (var ctx = new DataContext(_conn))
            {
                name = name.ToLower();

                return ctx.GetTable<ExecutableUnit>()
                    .Where(u => u.Name.ToLower().Contains(name) ||
                                u.Directory.ToLower().Contains(name) ||
                                u.FileName.ToLower().Contains(name))
                    .ToArray()
                    .OrderBy(u => MakeRank(u, name));
            }
        }

        public async Task UpdateIndexAsync(IEnumerable<string> targetFolders)
        {
            Close();

            var dir = Path.GetDirectoryName(_dataBaseFile);
            if (dir != null)
                Directory.CreateDirectory(dir);

            await Crawler.ExecuteAsync(_dataBaseFile, targetFolders);

            Open();
        }

        private void Open()
        {
            var sb = new SQLiteConnectionStringBuilder
            {
                DataSource = _dataBaseFile
            };

            _conn = new SQLiteConnection(sb.ToString());
            _conn.Open();
        }

        private void Close()
        {
            _conn?.Dispose();
            _conn = null;
        }

        // ReSharper disable PossibleNullReferenceException
        private static int MakeRank(ExecutableUnit u, string name)
        {
            var unitNameLength = Math.Min(u.Name.Length, 9999);

            Func<int, string, int> makeRankSub = (rankBase, target) =>
            {
                target = target.ToLower();

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

            var rankName = makeRankSub(++b, u.Name);
            if (rankName != int.MaxValue)
                return rankName;

            var rankFileName = makeRankSub(++b, Path.GetFileNameWithoutExtension(u.FileName));
            if (rankFileName != int.MaxValue)
                return rankFileName;

            var rankDir = makeRankSub(++b, u.Directory);
            if (rankDir != int.MaxValue)
                return rankDir;

            return int.MaxValue;
        }

        // ReSharper restore PossibleNullReferenceException
    }
}