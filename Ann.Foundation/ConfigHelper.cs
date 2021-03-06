﻿using System.IO;
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

        public static T ReadConfig<T>(Category category, string dirPath) where T : new()
        {
            try
            {
                var filePath = MakeFilePath(category, dirPath);

                if (File.Exists(filePath) == false)
                    return new T();

                using (var reader = new StringReader(File.ReadAllText(filePath)))
                {
                    var config = new DeserializerBuilder().IgnoreUnmatchedProperties().Build()
                        .Deserialize<T>(reader);

                    // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                    if (config == null)
                        config = new T();

                    return config;
                }
            }
            catch
            {
                return new T();
            }
        }

        public static void WriteConfig<T>(Category category, string dirPath, T config)
        {
            using (var writer = new StringWriter())
            {
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(writer, config);

                var filePath = MakeFilePath(category, dirPath);

                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, writer.ToString());
            }
        }

        public static string MakeFilePath(Category category, string dirPath) =>
            Path.Combine(
                dirPath,
                $"{AssemblyConstants.Product}.{category}.yaml");
    }
}