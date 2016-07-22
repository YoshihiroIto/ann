using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using YamlDotNet.Serialization;

namespace Ann
{
    public class App : DisposableModelBase
    {
        public static App Instance { get; } = new App();

        public Config.App Config { get; private set; }

        private HashSet<string> _highPriorities = new HashSet<string>();

        private readonly ExecutableUnitDataBase _dataBase;

        private static string IndexFilePath => Path.Combine(ConfigDirPath, "index.dat");

        public event EventHandler HighPriorityChanged;
        public event EventHandler ShortcutKeyChanged;

        #region IndexOpeningResult

        private IndexOpeningResults _IndexOpeningResult;

        public IndexOpeningResults IndexOpeningResult
        {
            get { return _IndexOpeningResult; }
            set { SetProperty(ref _IndexOpeningResult, value); }
        }

        #endregion

        public static void Initialize()
        {
            Instance.OpenIndex();
        }

        public static void Destory()
        {
            Instance.SaveConfig();
            Instance.Dispose();
        }

        public void InvokeShortcutKeyChanged() =>
            ShortcutKeyChanged?.Invoke(this, EventArgs.Empty);

        public bool IsHighPriority(string path) => _highPriorities.Contains(path);

        public bool AddHighPriorityPath(string path)
        {
            if (_highPriorities.Add(path) == false)
                return false;

            SaveConfig();
            HighPriorityChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public bool RemoveHighPriorityPath(string path)
        {
            if (_highPriorities.Remove(path) == false)
                return false;

            SaveConfig();
            HighPriorityChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void OpenIndex()
        {
            IndexOpeningResult = _dataBase.OpenIndex();
        }

        public async Task UpdateIndexAsync()
        {
            var targetFolders = Config.TargetFolder.Folders.ToList();

            if (Config.TargetFolder.IsIncludeSystemFolder)
                targetFolders.Add(new Config.Path(Constants.SystemFolder));

            if (Config.TargetFolder.IsIncludeSystemX86Folder)
                targetFolders.Add(new Config.Path(Constants.SystemX86Folder));

            if (Config.TargetFolder.IsIncludeProgramsFolder)
                targetFolders.Add(new Config.Path(Constants.ProgramsFolder));

            if (Config.TargetFolder.IsIncludeProgramFilesFolder)
                targetFolders.Add(new Config.Path(Constants.ProgramFilesFolder));

            if (Config.TargetFolder.IsIncludeProgramFilesX86Folder)
                targetFolders.Add(new Config.Path(Constants.ProgramFilesX86Folder));

            IndexOpeningResult = await _dataBase.UpdateIndexAsync(
                targetFolders
                .Select(f => Environment.ExpandEnvironmentVariables(f.Value))
                .Distinct()
                .Where(Directory.Exists));
        }

        public IEnumerable<ExecutableUnit> FindExecutableUnit(string name) =>
            _dataBase
                .Find(name)
                .OrderByDescending(u => IsHighPriority(u.Path))
                .Take(100);

        private App()
        {
            _dataBase = new ExecutableUnitDataBase(IndexFilePath);

            LoadConfig();
        }

        #region config

        public static string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(dir, CompanyName, ProductName);
            }
        }

        public static string ConfigFilePath => Path.Combine(ConfigDirPath, ProductName + ".yaml");

        private static string CompanyName =>
            ((AssemblyCompanyAttribute) Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false))
                .Company;

        private static string ProductName =>
            ((AssemblyProductAttribute) Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), false))
                .Product;

        private void LoadConfig()
        {
            Config = ReadConfigIfExist();

            _highPriorities = Config.HighPriorities == null
                ? new HashSet<string>()
                : new HashSet<string>(Config.HighPriorities);
        }

        private static Config.App ReadConfigIfExist()
        {
            if (File.Exists(ConfigFilePath) == false)
                return new Config.App();

            using (var reader = new StringReader(File.ReadAllText(ConfigFilePath)))
                return new Deserializer().Deserialize<Config.App>(reader);
        }

        public void SaveConfig()
        {
            Config.HighPriorities = new ObservableCollection<string>(_highPriorities);

            using (var writer = new StringWriter())
            {
                new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, Config);
                Directory.CreateDirectory(ConfigDirPath);
                File.WriteAllText(ConfigFilePath, writer.ToString());
            }
        }

        #endregion
    }
}