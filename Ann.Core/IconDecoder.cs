using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Ann.Foundation;
using Jewelry.Collections;
using Microsoft.WindowsAPICodePack.Shell;

namespace Ann.Core
{
    public class IconDecoder
    {
        public ImageSource GetIcon(string path)
        {
            if (File.Exists(path) == false)
                return null;

            return _IconCache.GetOrAdd(path, p =>
            {
                using (var file = ShellFile.FromFilePath(p))
                {
                    file.Thumbnail.CurrentSize = IconSize;

                    var bi = file.Thumbnail.BitmapSource;
                    if (bi.CanFreeze && bi.IsFrozen == false)
                        bi.Freeze();

                    return bi;
                }
            });
        }

        public class RefSize
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
                    var source = PresentationSource.FromVisual(Application.Current.MainWindow);

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

        private readonly LruCache<string, ImageSource> _IconCache = new LruCache<string, ImageSource>(256, false);
    }
}