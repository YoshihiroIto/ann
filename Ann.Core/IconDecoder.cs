using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ann.Foundation.Mvvm;
using Jewelry.Collections;
using Microsoft.WindowsAPICodePack.Shell;

namespace Ann.Core
{
    public class IconDecoder : ModelBase
    {
        #region IconCacheSize

        private int _IconCacheSize;

        public int IconCacheSize
        {
            get { return _IconCacheSize; }
            set
            {
                value = Math.Max(value, 0);

                if (SetProperty(ref _IconCacheSize, value))
                {
                    _IconCache = IconCacheSize == 0
                        ? null
                        : new LruCache<string, ImageBrush>(_IconCacheSize, false);
                }
            }
        }

        #endregion

        private readonly string _iconsDirPath;

        public IconDecoder(string configDirPath)
        {
            _iconsDirPath = System.IO.Path.Combine(configDirPath, "icons");

            Directory.CreateDirectory(_iconsDirPath);
        }

        public ImageBrush GetIcon(string path)
        {
            var i = _IconCache?.Get(path);
            if (i != null)
                return i;

            if (File.Exists(path) == false)
                return null;

            var ext = System.IO.Path.GetExtension(path)?.ToLower();

            if (string.IsNullOrEmpty(ext))
                return null;

            if (_IconShareFileExt.Contains(ext))
            {
                ImageBrush icon;
                if (_ShareIconCache.TryGetValue(ext, out icon))
                    return icon;

                icon = DecodeIcon(path);
                _ShareIconCache.Add(ext, icon);
                return icon;
            }

            return _IconCache == null
                ? DecodeIcon(path)
                : _IconCache.GetOrAdd(path, DecodeIcon);
        }

        public void ClearCache()
        {
            _IconCache?.Clear();
            _ShareIconCache?.Clear();

            if (Directory.Exists(_iconsDirPath))
                foreach (var f in Directory.EnumerateFiles(_iconsDirPath, "*.*", SearchOption.AllDirectories))
                    File.Delete(f);
        }

        private ImageBrush DecodeIcon(string path)
        {
            var hash = GenerateHash(path);

            var iconCacheDirPath = System.IO.Path.Combine(_iconsDirPath, $"{hash[0]}{hash[1]}");
            var iconCacheFilePath = System.IO.Path.Combine(iconCacheDirPath, hash);

            try
            {
                if (File.Exists(iconCacheFilePath))
                {
                    using (var stream = File.OpenRead(iconCacheFilePath))
                    {
                        var bmpImage = new BitmapImage();

                        bmpImage.BeginInit();
                        bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                        bmpImage.StreamSource = stream;
                        bmpImage.EndInit();

                        var b = new ImageBrush(bmpImage);
                        if (b.CanFreeze && b.IsFrozen == false)
                            b.Freeze();

                        return b;
                    }
                }
            }
            catch
            {
                // ignored
            }

            using (var file = ShellFile.FromFilePath(path))
            {
                file.Thumbnail.CurrentSize = IconSize;

                var bi = file.Thumbnail.BitmapSource;
                if (bi.CanFreeze && bi.IsFrozen == false)
                    bi.Freeze();

                var b = new ImageBrush(bi);
                if (b.CanFreeze && b.IsFrozen == false)
                    b.Freeze();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));

                Directory.CreateDirectory(iconCacheDirPath);
                using (var fs = new FileStream(iconCacheFilePath, FileMode.Create))
                    encoder.Save(fs);

                return b;
            }
        }

        private static string GenerateHash(string srcStr)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(srcStr));

            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        private class RefSize
        {
            public Size Size;
        }

        private static RefSize _IconRefSize;

        private static Size IconSize
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref _IconRefSize, () =>
                {
                    Window w = null;
                    try
                    {
                        w = Application.Current?.MainWindow;
                    }
                    catch
                    {
                        // ignored
                    }

                    if (w == null)
                        return new RefSize
                        {
                            Size =
                            {
                                Width = Constants.IconSize,
                                Height = Constants.IconSize
                            }
                        };

                    var source = PresentationSource.FromVisual(w);
                    if (source?.CompositionTarget == null)
                        return new RefSize
                        {
                            Size =
                            {
                                Width = Constants.IconSize,
                                Height = Constants.IconSize
                            }
                        };

                    return new RefSize
                    {
                        Size =
                        {
                            Width = Constants.IconSize*source.CompositionTarget.TransformToDevice.M11,
                            Height = Constants.IconSize*source.CompositionTarget.TransformToDevice.M22
                        }
                    };
                });

                return _IconRefSize.Size;
            }
        }

        private LruCache<string, ImageBrush> _IconCache;
        private readonly Dictionary<string, ImageBrush> _ShareIconCache = new Dictionary<string, ImageBrush>();

        private readonly HashSet<string> _IconShareFileExt = new HashSet<string>
        {
            ".bat",
            ".cmd",
            ".com",
            ".vbs",
            ".vbe",
            ".js",
            ".jse",
            ".wsf",
            ".wsh"
        };
    }
}