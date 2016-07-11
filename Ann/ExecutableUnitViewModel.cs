using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Jewelry.Collections;
using Microsoft.WindowsAPICodePack.Shell;

namespace Ann
{
    public class ExecutableUnitViewModel : ViewModelBase
    {
        #region Name

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        #endregion

        #region Path

        private string _Path;

        public string Path
        {
            get { return _Path; }
            set { SetProperty(ref _Path, value); }
        }

        #endregion

        #region Icon

        public ImageSource Icon
        {
            get
            {
                if (File.Exists(Path) == false)
                    return null;

                return IconCache.GetOrAdd(Path, path =>
                {
                    using (var file = ShellFile.FromFilePath(Path))
                    {
                        var thumbnail = file.Thumbnail;

#if false
                        const int iconSize = 48;
                        var mediumBitmapSource = thumbnail.MediumBitmapSource;
                        if ((int)mediumBitmapSource.Width == iconSize)
                            return mediumBitmapSource;

                        var largeBitmapSource = thumbnail.LargeBitmapSource;
                        if ((int) largeBitmapSource.Width == iconSize)
                            return largeBitmapSource;

                        thumbnail.CurrentSize = new Size(iconSize, iconSize);
#endif
                        return thumbnail.BitmapSource;
                    }
                });
            }
        }

#endregion

        public bool IsHighPriority
        {
            get { return App.Instance.IsHighPriority(Path); }
            set
            {
                if (value)
                {
                    if (App.Instance.AddHighPriorityPath(Path))
                        RaisePropertyChanged();
                }
                else
                {
                    if (App.Instance.RemoveHighPriorityPath(Path))
                        RaisePropertyChanged();
                }
            }
        }

        public ExecutableUnitViewModel(ExecutableUnit model)
        {
            Debug.Assert(model != null);

            Name = string.IsNullOrWhiteSpace(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;
        }
        
        private static readonly LruCache<string, ImageSource> IconCache = new LruCache<string, ImageSource>(512, false);
    }
}