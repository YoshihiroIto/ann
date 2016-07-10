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

        private static string ConfigFilePath
        {
            get
            {
                var loc = Assembly.GetEntryAssembly().Location;
                var dir = Path.GetDirectoryName(loc) ?? string.Empty;
                return Path.Combine(dir, "Ann.yaml");
            }
        }

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
            }
        }

        private void SaveConfig()
        {
            var config = new Config.App
            {
                HighPriorities = _highPriorities.ToArray()
            };

            using (var writer = new StringWriter())
            {
                new Serializer().Serialize(writer, config);

                File.WriteAllText(ConfigFilePath, writer.ToString());
            }
        }
    }
}