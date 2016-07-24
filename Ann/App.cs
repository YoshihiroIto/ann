using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;
using YamlDotNet.Serialization;
using Path = Ann.Core.Path;

namespace Ann
{
    public class App : DisposableModelBase
    {
        public static App Instance { get; } = new App();

        public Config.App Config { get; private set; }

        public event EventHandler PriorityFilesChanged;
        public event EventHandler ShortcutKeyChanged;

        public void InvokePriorityFilesChanged() => PriorityFilesChanged?.Invoke(this, EventArgs.Empty);
        public void InvokeShortcutKeyChanged() => ShortcutKeyChanged?.Invoke(this, EventArgs.Empty);

        private HashSet<string> _priorityFiles = new HashSet<string>();
        private readonly ExecutableUnitDataBase _dataBase;
        private static string IndexFilePath => System.IO.Path.Combine(ConfigDirPath, "index.dat");

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

        public bool IsPriorityFile(string path) => _priorityFiles.Contains(path);

        public bool AddPriorityFile(string path)
        {
            if (_priorityFiles.Contains(path))
                return false;

            Config.PriorityFiles.Add(new Path(path));
            return true;
        }

        public bool RemovePriorityFile(string path)
        {
            if (_priorityFiles.Contains(path) == false)
                return false;

            Config.PriorityFiles.Remove(new Path(path));
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
                targetFolders.Add(new Path(Constants.SystemFolder));

            if (Config.TargetFolder.IsIncludeSystemX86Folder)
                targetFolders.Add(new Path(Constants.SystemX86Folder));

            if (Config.TargetFolder.IsIncludeProgramsFolder)
                targetFolders.Add(new Path(Constants.ProgramsFolder));

            if (Config.TargetFolder.IsIncludeProgramFilesFolder)
                targetFolders.Add(new Path(Constants.ProgramFilesFolder));

            if (Config.TargetFolder.IsIncludeProgramFilesX86Folder)
                targetFolders.Add(new Path(Constants.ProgramFilesX86Folder));

            IndexOpeningResult = await _dataBase.UpdateIndexAsync(
                targetFolders
                .Select(f => Environment.ExpandEnvironmentVariables(f.Value))
                .Distinct()
                .Where(Directory.Exists));
        }

        public IEnumerable<ExecutableUnit> FindExecutableUnit(string name) =>
            _dataBase
                .Find(name)
                .OrderByDescending(u => IsPriorityFile(u.Path));

        private App()
        {
            _dataBase = new ExecutableUnitDataBase(IndexFilePath);

            LoadConfig();
        }

        public void RefreshPriorityFiles()
        {
            _priorityFiles = new HashSet<string>(Config.PriorityFiles.Select(p => p.Value));
        }

        #region config

        public static string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return System.IO.Path.Combine(dir, Constants.CompanyName, Constants.ProductName);
            }
        }

        public static string ConfigFilePath => System.IO.Path.Combine(ConfigDirPath, Constants.ProductName + ".yaml");

        private void LoadConfig()
        {
            Debug.Assert(Config == null);

            Config = ReadConfigIfExist();

            if (Config.PriorityFiles == null)
                Config.PriorityFiles = new ObservableCollection<Path>();

            RefreshPriorityFiles();

            Config.PriorityFiles.ObserveAddChanged()
                .Subscribe(p =>
                {
                    _priorityFiles.Add(p.Value);
                    SaveConfig();
                    InvokePriorityFilesChanged();
                })
                .AddTo(CompositeDisposable);

            Config.PriorityFiles.ObserveRemoveChanged()
                .Subscribe(p =>
                {
                    _priorityFiles.Remove(p.Value);
                    SaveConfig();
                    InvokePriorityFilesChanged();
                })
                .AddTo(CompositeDisposable);
        }

        private static Config.App ReadConfigIfExist()
        {
            if (File.Exists(ConfigFilePath) == false)
                return new Config.App();

            try
            {
                using (var reader = new StringReader(File.ReadAllText(ConfigFilePath)))
                    return new Deserializer().Deserialize<Config.App>(reader);
            }
            catch
            {
                return new Config.App();
            }
        }

        public void SaveConfig()
        {
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