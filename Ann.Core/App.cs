using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Core
{
    public class App : DisposableModelBase
    {
        public static App Instance { get; } = new App();

        public Config.App Config { get; private set; }
        public Config.MostRecentUsedList MruList { get; private set; }

        public event EventHandler PriorityFilesChanged;
        public event EventHandler ShortcutKeyChanged;

        public void InvokePriorityFilesChanged() => PriorityFilesChanged?.Invoke(this, EventArgs.Empty);
        public void InvokeShortcutKeyChanged() => ShortcutKeyChanged?.Invoke(this, EventArgs.Empty);

        private HashSet<string> _priorityFiles = new HashSet<string>();
        private readonly ExecutableUnitDataBase _dataBase;
        private static string IndexFilePath => System.IO.Path.Combine(Constants.ConfigDirPath, "index.dat");

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
        }

        public static void Destory()
        {
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

            var found = Config.PriorityFiles.First(p => p.Value == path);
            Config.PriorityFiles.Remove(found);
            return true;
        }

        public async Task OpenIndexAsync()
        {
            IndexOpeningResult = IndexOpeningResults.InOpening;
            IndexOpeningResult = await _dataBase.OpenIndexAsync();
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
                    .Where(Directory.Exists),
                Config.ExecutableFileExts);
        }

        public IEnumerable<ExecutableUnit> FindExecutableUnit(string name) =>
            _dataBase
                .Find(name, Config.ExecutableFileExts)
                .OrderBy(u => MakeOrder(u.Path));

        private const int MaxMruCount = 50;
        private readonly Dictionary<string, int> _mruOrders = new Dictionary<string, int>();

        private int MakeOrder(string path)
        {
            if (IsPriorityFile(path))
                return 0;

            int order;
            if (_mruOrders.TryGetValue(path, out order))
                return 1 + order;

            return 1 + MaxMruCount;
        }

        public void RefreshPriorityFiles()
        {
            _priorityFiles = new HashSet<string>(Config.PriorityFiles.Select(p => p.Value));
        }

        public async Task RunAsync(string appPath, bool isRunAsAdmin)
        {
            await ProcessHelper.RunAsync(appPath, string.Empty, isRunAsAdmin);

            await Task.Run(() =>
            {
                MruList.AppPath.Remove(appPath);
                MruList.AppPath.Insert(0, appPath);

                while (MruList.AppPath.Count > MaxMruCount)
                    MruList.AppPath.RemoveAt(MruList.AppPath.Count - 1);
                
                SaveMru();

                _mruOrders.Clear();
                MruList.AppPath.ForEach((path, index) => _mruOrders[path] = index);
            });
        }

        private App()
        {
            _dataBase = new ExecutableUnitDataBase(IndexFilePath);

            LoadConfig();
        }

        #region config

        private void LoadConfig()
        {
            Debug.Assert(Config == null);
            Debug.Assert(MruList == null);

            {
                Config = ConfigHelper.ReadConfig<Config.App>(ConfigHelper.Category.App, Constants.ConfigDirPath);

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

            {
                MruList = ConfigHelper.ReadConfig<Config.MostRecentUsedList>(ConfigHelper.Category.MostRecentUsedList, Constants.ConfigDirPath);
                MruList.AppPath.ForEach((p, index) => _mruOrders[p] = index);
            }
        }

        public void SaveConfig()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.App, Constants.ConfigDirPath, Config);
        }

        public void SaveMru()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.MostRecentUsedList, Constants.ConfigDirPath, MruList);
        }

        #endregion
    }
}