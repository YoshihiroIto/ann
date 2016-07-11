using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ann.Core
{
    public static class Crawler
    {
        public static async Task<bool> ExecuteAsync(string databaseFilePath, IEnumerable<string> targetFolders)
        {
            await Task.Run(() =>
            {
                var sb = new SQLiteConnectionStringBuilder { DataSource = databaseFilePath };

                using (var conn = new SQLiteConnection(sb.ToString()))
                {
                    conn.Open();

                    CreateTable(conn);
                    DeleteAll(conn);

                    using (var ctx = new DataContext(conn))
                    {
                        var executableExts = MakeExecutableExts();

                        ctx.GetTable<ExecutableUnit>().InsertAllOnSubmit(
                            targetFolders.SelectMany(targetFolder =>
                                EnumerateAllFiles(targetFolder)
                                    .Where(f => executableExts.Contains(Path.GetExtension(f)?.ToLower()))
                                    .Select(f =>
                                    {
                                        var fvi = FileVersionInfo.GetVersionInfo(f);

                                        var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                                            ? Path.GetFileNameWithoutExtension(f)
                                            : fvi.FileDescription;

                                        return new ExecutableUnit
                                        {
                                            Path = f,
                                            Name = name,
                                        };
                                    })
                                ));

                        ctx.SubmitChanges();
                    }

                    conn.Close();
                }
            });

            return true;
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
#if false
            var pathext = Environment.GetEnvironmentVariable("PATHEXT");

            return pathext == null
                ? new HashSet<string> {".exe"}
                : new HashSet<string>(pathext.Split(';').Select(p => p.ToLower()));
#else
            return new HashSet<string> {".exe"};
#endif
        }

        private static void CreateTable(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"create table if not exists {nameof(ExecutableUnit)} (Path NVARCHAR PRIMARY KEY, Name NVARCHAR)";

                cmd.ExecuteNonQuery();
            }
        }

        private static void DeleteAll(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"delete from {nameof(ExecutableUnit)}";

                cmd.ExecuteNonQuery();
            }
        }
    }
}