using System.Diagnostics;
using Ann.Foundation;

namespace Ann.Core
{
    public class ConfigHolder
    {
        public string ConfigDirPath { get; }

        public Config.App Config { get; private set; }
        public Config.MostRecentUsedList MruList { get; private set; }
        public Config.MainWindow MainWindow { get; private set; }

        public ConfigHolder(string configDirPath)
        {
            Debug.Assert(string.IsNullOrEmpty(configDirPath) == false);

            ConfigDirPath = configDirPath;
            LoadConfig();
        }

        private void LoadConfig()
        {
            Debug.Assert(Config == null);
            Debug.Assert(MruList == null);

            Config = ConfigHelper.ReadConfig<Config.App>(ConfigHelper.Category.App, ConfigDirPath);
            MruList = ConfigHelper.ReadConfig<Config.MostRecentUsedList>(ConfigHelper.Category.MostRecentUsedList, ConfigDirPath);
            MainWindow = ConfigHelper.ReadConfig<Config.MainWindow>(ConfigHelper.Category.MainWindow, ConfigDirPath);
        }

        public void SaveConfig()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.App, ConfigDirPath, Config);
        }

        public void SaveMru()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.MostRecentUsedList, ConfigDirPath, MruList);
        }

        public void SaveMainWindow()
        {
            ConfigHelper.WriteConfig(ConfigHelper.Category.MainWindow, ConfigDirPath, MainWindow);
        }
    }
}