using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Core;
using Ann.Foundation.Mvvm;

namespace Ann.Config
{
    public class App : ModelBase
    {
        #region TargetFolder

        private TargetFolder _TargetFolder = new TargetFolder();

        public TargetFolder TargetFolder
        {
            get { return _TargetFolder; }
            set { SetProperty(ref _TargetFolder, value); }
        }

        #endregion

        #region PriorityFiles

        private ObservableCollection<Path> _PriorityFiles = new ObservableCollection<Path>();

        public ObservableCollection<Path> PriorityFiles
        {
            get { return _PriorityFiles; }
            set { SetProperty(ref _PriorityFiles, value); }
        }
        #endregion

        #region MainWindow

        private MainWindow _MainWindow = new MainWindow();

        public MainWindow MainWindow
        {
            get { return _MainWindow; }
            set { SetProperty(ref _MainWindow, value); }
        }

        #endregion
    }

    public class TargetFolder : ModelBase
    {
        #region IsIncludeSystemFolder

        private bool _IsIncludeSystemFolder = true;

        public bool IsIncludeSystemFolder
        {
            get { return _IsIncludeSystemFolder; }
            set { SetProperty(ref _IsIncludeSystemFolder, value); }
        }

        #endregion

        #region IsIncludeSystemX86Folder

        private bool _IsIncludeSystemX86Folder = true;

        public bool IsIncludeSystemX86Folder
        {
            get { return _IsIncludeSystemX86Folder; }
            set { SetProperty(ref _IsIncludeSystemX86Folder, value); }
        }

        #endregion

        #region IsIncludeProgramsFolder

        private bool _IsIncludeProgramsFolder = true;

        public bool IsIncludeProgramsFolder
        {
            get { return _IsIncludeProgramsFolder; }
            set { SetProperty(ref _IsIncludeProgramsFolder, value); }
        }

        #endregion

        #region IsIncludeProgramFilesFolder

        private bool _IsIncludeProgramFilesFolder = true;

        public bool IsIncludeProgramFilesFolder
        {
            get { return _IsIncludeProgramFilesFolder; }
            set { SetProperty(ref _IsIncludeProgramFilesFolder, value); }
        }

        #endregion

        #region IsIncludeProgramFilesX86Folder

        private bool _IsIncludeProgramFilesX86Folder = true;

        public bool IsIncludeProgramFilesX86Folder
        {
            get { return _IsIncludeProgramFilesX86Folder; }
            set { SetProperty(ref _IsIncludeProgramFilesX86Folder, value); }
        }

        #endregion

        #region Folders

        private ObservableCollection<Path> _Folders = new ObservableCollection<Path>();

        public ObservableCollection<Path> Folders
        {
            get { return _Folders; }
            set { SetProperty(ref _Folders, value); }
        }
        #endregion
    }

    public class MainWindow : ModelBase
    {
        #region Left

        private double _Left = double.NaN;

        public double Left
        {
            get { return _Left; }
            set { SetProperty(ref _Left, value); }
        }

        #endregion

        #region Top

        private double _Top = double.NaN;

        public double Top
        {
            get { return _Top; }
            set { SetProperty(ref _Top, value); }
        }

        #endregion

        #region MaxCandidateLinesCount

        private int _MaxCandidateLinesCount = Constants.DefaultMaxCandidateLinesCount;

        public int MaxCandidateLinesCount
        {
            get { return _MaxCandidateLinesCount; }
            set { SetProperty(ref _MaxCandidateLinesCount, value); }
        }

        #endregion

        #region ShortcutKeys

        private ShortcutKeys _ShortcutKeys = new ShortcutKeys();

        public ShortcutKeys ShortcutKeys
        {
            get { return _ShortcutKeys; }
            set { SetProperty(ref _ShortcutKeys, value); }
        }

        #endregion
    }

    public class ShortcutKeys : ModelBase
    {
        #region Activate

        private ShortcutKey _Activate =
            new ShortcutKey {Key = Key.None};

        public ShortcutKey Activate
        {
            get { return _Activate; }
            set { SetProperty(ref _Activate, value); }
        }

        #endregion

        #region Hide

        private ObservableCollection<ShortcutKey> _Hide = new ObservableCollection<ShortcutKey>();

        public ObservableCollection<ShortcutKey> Hide
        {
            get { return _Hide; }
            set { SetProperty(ref _Hide, value); }
        }

        #endregion
    }
}