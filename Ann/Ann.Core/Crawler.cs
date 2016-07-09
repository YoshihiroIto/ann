using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Ann.Core
{
    public static class Crawler
    {
        public static void Execute(string databaseFile, IEnumerable<string> targetFolders)
        {
            if (File.Exists(databaseFile))
                File.Delete(databaseFile);

            var sb = new SQLiteConnectionStringBuilder
            {
                DataSource = databaseFile
            };

            using (var conn = new SQLiteConnection(sb.ToString()))
            {
                conn.Open();
                CreateTable(conn);

                using (var ctx = new DataContext(conn))
                {
                    var executableExts = MakeExecutableExts();

                    var count = 0;

                    ctx.GetTable<ExecutableUnit>().InsertAllOnSubmit(
                        targetFolders.SelectMany(targetFolder =>
                            EnumerateAllFiles(targetFolder)
                                .Where(f => executableExts.Contains(Path.GetExtension(f)?.ToLower()))
                                .Select(f => new ExecutableUnit
                                {
                                    Id = count ++,
                                    Name = Path.GetFileNameWithoutExtension(f),
                                    Path = f
                                })
                            ));

                    ctx.SubmitChanges();
                }

                conn.Close();
            }
        }

        public static IEnumerable<string> EnumerateAllFiles(string path)
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

        private static HashSet<string> MakeExecutableExts()
        {
            var pathext = Environment.GetEnvironmentVariable("PATHEXT");

            return pathext == null
                ? new HashSet<string>{".exe"}
                : new HashSet<string>(pathext.Split(';').Select(p => p.ToLower()));
        }

        private static void CreateTable(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"create table {nameof(ExecutableUnit)} (Id INT PRIMARY KEY, Name NVARCHAR, Path NVARCHAR)";

                cmd.ExecuteNonQuery();
            }
        }
    }
}