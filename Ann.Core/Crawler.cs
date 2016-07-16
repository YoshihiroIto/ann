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

                    using (var ctx = new DataContext(conn))
                    {
                        var executableExts = new HashSet<string> {".exe", ".lnk"};

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
                                            Directory = Path.GetDirectoryName(f) ?? string.Empty,
                                            FileName = Path.GetFileNameWithoutExtension(f)
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

        private static void CreateTable(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"drop table if exists {nameof(ExecutableUnit)}";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"create table {nameof(ExecutableUnit)} (Path TEXT PRIMARY KEY, Name TEX, Directory TEXTT, FileName TEXT)";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    $"delete from {nameof(ExecutableUnit)}";

                cmd.ExecuteNonQuery();
            }
        }
    }
}