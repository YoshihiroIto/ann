using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;

namespace Ann.Foundation
{
    public static class ConfigHelper
    {
        public enum Category
        {
            App,
            MainWindow
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

        private static string CompanyName =>
            ((AssemblyCompanyAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false))
                .Company;

        private static string ProductName =>
            ((AssemblyProductAttribute) Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute), false))
                .Product;

        public static string ConfigDirPath
        {
            get
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(dir, CompanyName, ProductName);
            }
        }

        private static readonly IReadOnlyDictionary<Category, string> FilePaths = new Dictionary<Category, string>
        {
            {Category.App, Path.Combine(ConfigDirPath, ProductName + ".App.yaml")},
            {Category.MainWindow, Path.Combine(ConfigDirPath, ProductName + ".MainWindow.yaml")}
        };
    }
}