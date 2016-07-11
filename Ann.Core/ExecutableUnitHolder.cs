using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Ann.Core
{
    public class ExecutableUnitHolder : IDisposable
    {
        private readonly SQLiteConnection _conn;

        public ExecutableUnitHolder(string databaseFile)
        {
            if (File.Exists(databaseFile) == false)
                return;

            var sb = new SQLiteConnectionStringBuilder
            {
                DataSource = databaseFile
            };

            _conn = new SQLiteConnection(sb.ToString());
            _conn.Open();
        }

        public void Dispose()
        {
            _conn?.Dispose();
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

        // ReSharper disable PossibleNullReferenceException
        private static int MakeOrder(ExecutableUnit u, string name)
        {
            var filename = Path.GetFileNameWithoutExtension(u.Path);

            if (filename == name)
                return 0;

            if (filename.StartsWith(name))
                return 1;

            return filename.Contains(name) ? 2 : 3;
        }

        // ReSharper restore PossibleNullReferenceException
    }
}