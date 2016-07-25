using System.Collections.ObjectModel;
using System.Windows.Input;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Config
{
    public class App : ModelBase
    {
        #region ShortcutKeys

        private ShortcutKeys _ShortcutKeys = new ShortcutKeys();

        public ShortcutKeys ShortcutKeys
        {
            get { return _ShortcutKeys; }
            set { SetProperty(ref _ShortcutKeys, value); }
        }

        #endregion

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

        #region IconCacheSize

        private int _IconCacheSize = 256;

        public int IconCacheSize
        {
            get { return _IconCacheSize; }
            set { SetProperty(ref _IconCacheSize, value); }
        }

        #endregion

        #region CandidatesCensoringSize

        private int _CandidatesCensoringSize = 100;

        public int CandidatesCensoringSize
        {
            get { return _CandidatesCensoringSize; }
            set { SetProperty(ref _CandidatesCensoringSize, value); }
        }

        #endregion

        #region Culture

        private string _Culture = string.Empty;

        public string Culture
        {
            get { return _Culture; }
            set { SetProperty(ref _Culture, value); }
        }

        #endregion

        #region MaxCandidateLinesCount

        private int _MaxCandidateLinesCount = 8;

        public int MaxCandidateLinesCount
        {
            get { return _MaxCandidateLinesCount; }
            set { SetProperty(ref _MaxCandidateLinesCount, value); }
        }

        #endregion

        #region ExecutableFileExts

        private ObservableCollection<string> _ExecutableFileExts = new ObservableCollection<string>{"exe", "lnk"};

        public ObservableCollection<string> ExecutableFileExts
        {
            get { return _ExecutableFileExts; }
            set { SetProperty(ref _ExecutableFileExts, value); }
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

    public class ShortcutKey : ModelBase
    {
        #region Key

        private Key _Key;

        public Key Key
        {
            get { return _Key; }
            set { SetProperty(ref _Key, value); }
        }

        #endregion

        #region Modifiers

        private ModifierKeys _Modifiers;

        public ModifierKeys Modifiers
        {
            get { return _Modifiers; }
            set { SetProperty(ref _Modifiers, value); }
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