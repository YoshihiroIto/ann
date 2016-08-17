using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
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
                        : new LruCache<string, ImageSource>(_IconCacheSize, false);
                }
            }
        }

        #endregion

        public ImageSource GetIcon(string path)
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
                ImageSource icon;
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

        private static ImageSource DecodeIcon(string path)
        {
            using (var file = ShellFile.FromFilePath(path))
            {
                file.Thumbnail.CurrentSize = IconSize;

                var bi = file.Thumbnail.BitmapSource;
                if (bi.CanFreeze && bi.IsFrozen == false)
                    bi.Freeze();

                return bi;
            }
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

        private LruCache<string, ImageSource> _IconCache;
        private readonly Dictionary<string, ImageSource> _ShareIconCache = new Dictionary<string, ImageSource>();

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