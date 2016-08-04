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

        public static T ReadConfig<T>(Category category, string dirPath) where T : new()
        {
            var filePath = MakeFilePath(category, dirPath);

            if (File.Exists(filePath) == false)
                return new T();

            try
            {
                using (var reader = new StringReader(File.ReadAllText(filePath)))
                    return new Deserializer().Deserialize<T>(reader);
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
                new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, config);
                
                var filePath = MakeFilePath(category, dirPath);

                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, writer.ToString());
            }
        }

        private static string MakeFilePath(Category category, string dirPath) =>
            Path.Combine(dirPath, AssemblyConstants.Product + $".{category}.yaml" );
    }
}