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
            if (File.Exists(databaseFile) == false)
                return;

            _dataBaseFile = databaseFile;

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
                    .Where(u => u.Name.ToLower().Contains(name) || u.Path.ToLower().Contains(name))
                    .ToArray()
                    .OrderBy(u => MakeOrder(u, name))
                    .ToArray();
            }
        }
        
        public async Task UpdateIndexAsync()
        {
            Close();

            var dir = Path.GetDirectoryName(_dataBaseFile);
            if (dir != null)
                Directory.CreateDirectory(dir);

            await Crawler.ExecuteAsync(
                _dataBaseFile,
                new[]
                {
                    @"C:\Program Files",
                    @"C:\Program Files (x86)"
                });

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
        private static int MakeOrder(ExecutableUnit u, string name)
        {
            var filename = Path.GetFileNameWithoutExtension(u.Path).ToLower();

            if (filename == name)
                return 0;

            if (filename.StartsWith(name))
                return 1;

            return filename.Contains(name) ? 2 : 3;
        }
        // ReSharper restore PossibleNullReferenceException
    }
}