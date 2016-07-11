using System.Diagnostics;
using System.IO;
using System.Windows;
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
                        const double iconSize = 48;
                        file.Thumbnail.CurrentSize = new Size(iconSize*Scale.Width, iconSize*Scale.Height);

                        return file.Thumbnail.BitmapSource;
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

        public static Size Scale { get; set; }

        private static readonly LruCache<string, ImageSource> IconCache = new LruCache<string, ImageSource>(512, false);
    }
}