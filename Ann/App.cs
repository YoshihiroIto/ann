using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ann.Foundation.Mvvm;
using YamlDotNet.Serialization;

namespace Ann
{
    public class App : ModelBase
    {
        public static App Instance { get; } = new App();

        private HashSet<string> _highPriorities = new HashSet<string>();

        #region MainWindowLeft

        private double _MainWindowLeft;

        public double MainWindowLeft
        {
            get { return _MainWindowLeft; }
            set { SetProperty(ref _MainWindowLeft, value); }
        }

        #endregion

        #region MainWindowTop

        private double _MainWindowTop;

        public double MainWindowTop
        {
            get { return _MainWindowTop; }
            set { SetProperty(ref _MainWindowTop, value); }
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
        public bool AddHighPriorityPath(string path) => _highPriorities.Add(path);
        public bool RemoveHighPriorityPath(string path) => _highPriorities.Remove(path);

        private App()
        {
            LoadConfig();
        }

        public string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(dir, CompanyName, ProductName);
            }
        }

        public string ConfigFilePath => Path.Combine(ConfigDirPath, ProductName + ".yaml");

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
            if (File.Exists(ConfigFilePath) == false)
                return;

            using (var reader = new StringReader(File.ReadAllText(ConfigFilePath)))
            {
                var config = new Deserializer().Deserialize<Config.App>(reader);

                _highPriorities = config.HighPriorities == null
                    ? new HashSet<string>()
                    : new HashSet<string>(config.HighPriorities);

                MainWindowLeft = config.MainWindow?.Left ?? 0;
                MainWindowTop = config.MainWindow?.Top ?? 0;
            }
        }

        private void SaveConfig()
        {
            var config = new Config.App
            {
                HighPriorities = _highPriorities.ToArray(),
                MainWindow = new Config.MainWindow
                {
                    Left = MainWindowLeft,
                    Top = MainWindowTop
                }
            };

            using (var writer = new StringWriter())
            {
                new Serializer().Serialize(writer, config);
                Directory.CreateDirectory(ConfigDirPath);
                File.WriteAllText(ConfigFilePath, writer.ToString());
            }
        }
    }
}