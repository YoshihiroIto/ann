using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Ann.Foundation
{
    public static class ConfigHelper
    {
        public enum Category
        {
            App,
            MainWindow,
            MostRecentUsedList
        }

        public static T ReadConfig<T>(Category category) where T:new()
        {
            if (File.Exists(FilePaths[category]) == false)
                return new T();

            try
            {
                using (var reader = new StringReader(File.ReadAllText(FilePaths[category])))
                    return new Deserializer().Deserialize<T>(reader);
            }
            catch
            {
                return new T();
            }
        }

        public static void WriteConfig<T>(Category category, T config)
        {
            using (var writer = new StringWriter())
            {
                new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, config);
                Directory.CreateDirectory(ConfigDirPath);
                File.WriteAllText(FilePaths[category], writer.ToString());
            }
        }

        public static string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(dir, AssemblyConstants.CompanyName, AssemblyConstants.ProductName);
            }
        }

        private static readonly IReadOnlyDictionary<Category, string> FilePaths = new Dictionary<Category, string>
        {
            {Category.App, Path.Combine(ConfigDirPath, AssemblyConstants.ProductName + ".App.yaml")},
            {Category.MainWindow, Path.Combine(ConfigDirPath, AssemblyConstants.ProductName + ".MainWindow.yaml")},
            {Category.MostRecentUsedList, Path.Combine(ConfigDirPath, AssemblyConstants.ProductName + ".MostRecentUsedList.yaml")}
        };
    }
}