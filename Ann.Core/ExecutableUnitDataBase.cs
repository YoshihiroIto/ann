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
        public event EventHandler Opend;
        public event EventHandler Closed;

        private SQLiteConnection _conn;
        private bool _isOpend;

        private readonly string _dataBaseFile;

        public ExecutableUnitDataBase(string databaseFile)
        {
            _dataBaseFile = databaseFile;
            Opend += (_, __) => _isOpend = true;
            Closed += (_, __) => _isOpend = false;
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<ExecutableUnit> Find(string name)
        {
            if (name == null)
                return Enumerable.Empty<ExecutableUnit>();

            if (_conn == null)
                return Enumerable.Empty<ExecutableUnit>();

            if (_isOpend == false)
                return Enumerable.Empty<ExecutableUnit>();

            name = name.Trim();

            if (name == string.Empty)
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
            var dir = Path.GetDirectoryName(_dataBaseFile);
            if (dir != null)
                Directory.CreateDirectory(dir);

            await Crawler.ExecuteAsync(_conn, targetFolders);

            await Task.Run(() =>
            {
                using (var dst = new SQLiteConnection($"Data Source={_dataBaseFile};"))
                {
                    dst.Open();
                    _conn.BackupDatabase(dst, "main", "main", -1, null, 0);
                }
            });

            Opend?.Invoke(this, EventArgs.Empty);
        }

        public async Task OpenAsync()
        {
            await Task.Run(() =>
            {
                var sb = new SQLiteConnectionStringBuilder
                {
                    DataSource = ":memory:",
                    SyncMode = SynchronizationModes.Off,
                    JournalMode = SQLiteJournalModeEnum.Memory
                };

                _conn = new SQLiteConnection(sb.ToString());
                _conn.Open();

                if (File.Exists(_dataBaseFile) == false)
                    return;

                using (var src = new SQLiteConnection($"Data Source={_dataBaseFile};"))
                {
                    src.Open();
                    src.BackupDatabase(_conn, "main", "main", -1, null, 0);
                }

                Opend?.Invoke(this, EventArgs.Empty);
            });
        }

        private void Close()
        {
            _conn?.Dispose();
            _conn = null;

            Closed?.Invoke(this, EventArgs.Empty);
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

            var rankFileName = makeRankSub(++b, u.FileName);
            if (rankFileName != int.MaxValue)
                return rankFileName;

            var rankName = makeRankSub(++b, u.Name);
            if (rankName != int.MaxValue)
                return rankName;

            var rankDir = makeRankSub(++b, u.Directory);
            if (rankDir != int.MaxValue)
                return rankDir;

            return int.MaxValue;
        }
        // ReSharper restore PossibleNullReferenceException
    }
}