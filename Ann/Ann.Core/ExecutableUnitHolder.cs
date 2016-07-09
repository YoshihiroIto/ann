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
        private readonly DataContext _ctx;

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

            _ctx = new DataContext(_conn);
        }

        public void Dispose()
        {
            _ctx?.Dispose();
            _conn?.Dispose();
        }

        public IEnumerable<ExecutableUnit> Find(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Enumerable.Empty<ExecutableUnit>();

            // ReSharper disable once MergeConditionalExpression
            return _ctx == null
                ? Enumerable.Empty<ExecutableUnit>()
                : _ctx.GetTable<ExecutableUnit>().Where(u => u.Name.ToLower().Contains(name.ToLower()));
        }
    }
}