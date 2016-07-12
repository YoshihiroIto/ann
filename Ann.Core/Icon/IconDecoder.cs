using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Jewelry.Collections;
using Microsoft.WindowsAPICodePack.Shell;

namespace Ann.Core.Icon
{
    public class IconDecoder : IDisposable
    {
        public Size IconSize { get; set; }

        private readonly SQLiteConnection _conn;

        public IconDecoder(string databaseFile)
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

        public ImageSource GetIcon(string path)
        {
            if (File.Exists(path) == false)
                return null;

            return _IconCache.GetOrAdd(path, p =>
            {
                using (var file = ShellFile.FromFilePath(p))
                {
                    file.Thumbnail.CurrentSize = IconSize;
                    return file.Thumbnail.BitmapSource;
                }
            });
        }

        private readonly LruCache<string, ImageSource> _IconCache = new LruCache<string, ImageSource>(512, false);
    }
}