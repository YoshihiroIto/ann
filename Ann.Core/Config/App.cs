using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using YamlDotNet.Serialization;

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

        #region MaxCandidateLinesCount

        private int _MaxCandidateLinesCount = 10;

        public int MaxCandidateLinesCount
        {
            get { return _MaxCandidateLinesCount; }
            set { SetProperty(ref _MaxCandidateLinesCount, value); }
        }

        #endregion

        #region IsStartOnSystemStartup

        private bool _IsStartOnSystemStartup = true;

        public bool IsStartOnSystemStartup
        {
            get { return _IsStartOnSystemStartup; }
            set { SetProperty(ref _IsStartOnSystemStartup, value); }
        }

        #endregion

        #region Culture

        private string _Culture;

        public string Culture
        {
            get
            {
                if (_Culture != null)
                    return _Culture;

                var name = CultureInfo.CurrentUICulture.Name;
                var isoName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                if (Constants.SupportedCultures.Any(c => c.CultureName == name))
                    _Culture = name;
                else if (Constants.SupportedCultures.Any(c => c.CultureName == isoName))
                    _Culture = isoName;
                else
                    _Culture = "en";

                return _Culture;
            }
            set { SetProperty(ref _Culture, value); }
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

        #region ExecutableFileExts

        private ObservableCollection<string> _ExecutableFileExts = new ObservableCollection<string>
        {
            "exe",
            "lnk",
            "appref-ms",
            "bat",
            "cmd",
            "com",
            "vbs",
            "vbe",
            "js",
            "jse",
            "wsf",
            "wsh"
        };

        public ObservableCollection<string> ExecutableFileExts
        {
            get { return _ExecutableFileExts; }
            set { SetProperty(ref _ExecutableFileExts, value); }
        }

        #endregion

        #region GitHubPersonalAccessToken

        private string _GitHubPersonalAccessToken = string.Empty;

        public string GitHubPersonalAccessToken
        {
            get { return _GitHubPersonalAccessToken; }
            set { SetProperty(ref _GitHubPersonalAccessToken, value); }
        }

        #endregion

        #region Translator

        private Translator _Translator = new Translator();

        public Translator Translator
        {
            get { return _Translator; }
            set { SetProperty(ref _Translator, value); }
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

        #region IsIncludeCommonStartMenu

        private bool _IsIncludeCommonStartMenu = true;

        public bool IsIncludeCommonStartMenu
        {
            get { return _IsIncludeCommonStartMenu; }
            set { SetProperty(ref _IsIncludeCommonStartMenu, value); }
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

        [YamlIgnore]
        public string Text
        {
            get
            {
                if (Key == Key.None)
                    return string.Empty;

                var sb = new StringBuilder();

                if ((Modifiers & ModifierKeys.Control) != 0)
                    sb.Append("Ctrl + ");
                if ((Modifiers & ModifierKeys.Alt) != 0)
                    sb.Append("Alt + ");
                if ((Modifiers & ModifierKeys.Shift) != 0)
                    sb.Append("Shift + ");

                sb.Append(Key);

                return sb.ToString();
            }
        }
    }

    public class ShortcutKeys : ModelBase
    {
        #region Activate

        private ShortcutKey _Activate =
            new ShortcutKey {Key = Key.None, Modifiers = ModifierKeys.None};

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

    public class Translator : ModelBase
    {
        #region MicrosoftTranslatorClientId

        private string _MicrosoftTranslatorClientId = string.Empty;

        public string MicrosoftTranslatorClientId
        {
            get { return _MicrosoftTranslatorClientId; }
            set { SetProperty(ref _MicrosoftTranslatorClientId, value); }
        }

        #endregion

        #region MicrosoftTranslatorClientSecret

        private string _MicrosoftTranslatorClientSecret = string.Empty;

        public string MicrosoftTranslatorClientSecret
        {
            get { return _MicrosoftTranslatorClientSecret; }
            set { SetProperty(ref _MicrosoftTranslatorClientSecret, value); }
        }

        #endregion

        #region TranslatorSet

        private ObservableCollection<TranslatorSet> _TranslatorSet = new ObservableCollection<TranslatorSet>
        {
            new TranslatorSet
            {
                Keyword = "ja",
                To = TranslateService.LanguageCodes.ja
            },
            new TranslatorSet
            {
                Keyword = "en",
                To = TranslateService.LanguageCodes.en
            }
        };

        public ObservableCollection<TranslatorSet> TranslatorSet
        {
            get { return _TranslatorSet; }
            set { SetProperty(ref _TranslatorSet, value); }
        }

        #endregion
    }

    public class TranslatorSet : ModelBase
    {
        #region Keyword

        private string _Keyword;

        public string Keyword
        {
            get { return _Keyword; }
            set { SetProperty(ref _Keyword, value); }
        }

        #endregion

        #region From

        private TranslateService.LanguageCodes _From = TranslateService.LanguageCodes.AutoDetect;

        public TranslateService.LanguageCodes From
        {
            get { return _From; }
            set { SetProperty(ref _From, value); }
        }

        #endregion

        #region To

        private TranslateService.LanguageCodes _To;

        public TranslateService.LanguageCodes To
        {
            get { return _To; }
            set { SetProperty(ref _To, value); }
        }

        #endregion
    }
}