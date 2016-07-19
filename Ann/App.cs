using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;
using YamlDotNet.Serialization;

namespace Ann
{
    public class App : DisposableModelBase
    {
        public static App Instance { get; } = new App();

        public Config.App Config { get; private set; }

        private HashSet<string> _highPriorities = new HashSet<string>();

        private readonly ExecutableUnitDataBase _dataBase;

        private static string IndexDbFilePath => Path.Combine(ConfigDirPath, "Index.db");
        public static string IconCacheFilePath => Path.Combine(ConfigDirPath, "IconCache.db");

        public event EventHandler HighPriorityChanged;
        public event EventHandler ActivateShortcutKeyChanged;

        #region IsEnabledIndex

        private bool _IsEnabledIndex;

        public bool IsEnabledIndex
        {
            get { return _IsEnabledIndex; }
            set { SetProperty(ref _IsEnabledIndex, value); }
        }

        #endregion

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            Instance.SaveConfig();
            Instance.Dispose();
        }

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

            await _dataBase.UpdateIndexAsync(
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
            _dataBase = new ExecutableUnitDataBase(IndexDbFilePath).AddTo(CompositeDisposable);
            _dataBase.Opend += (_, __) => IsEnabledIndex = true;
            _dataBase.Closed += (_, __) => IsEnabledIndex = false;

            LoadConfig();

            Observable
                .Merge(Config.MainWindow.ShortcutKeys.Activate.ObserveProperty(x => x.Key).ToUnit())
                .Merge(Config.MainWindow.ShortcutKeys.Activate.ObserveProperty(x => x.Modifiers).ToUnit())
                .Subscribe(_ => ActivateShortcutKeyChanged?.Invoke(this, EventArgs.Empty))
                .AddTo(CompositeDisposable);
        }

        public async Task OpenIndexAsync()
        {
            await _dataBase.OpenAsync();
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