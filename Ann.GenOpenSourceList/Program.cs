using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ann.Foundation;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Ann.GenOpenSourceList
{
    class Program
    {
        static void Main()
        {
            var packagesConfigPaths = Directory.EnumerateFiles(
                @"..\..\..\",
                "packages.config", SearchOption.AllDirectories);

            var yaml = Generate(packagesConfigPaths);
            File.WriteAllText(@"..\..\..\Ann.Core\OpenSourceList.yaml", yaml);
        }

        private static string Generate(IEnumerable<string> packagesConfigPaths)
        {
            using (new TimeMeasure())
            {
                var packageNames = packagesConfigPaths
                    .Select(XElement.Load)
                    .SelectMany(p => p.Elements("package"))
                    .Select(p => p.Attribute("id").Value)
                    .Distinct();

                var genPackageTasks = packageNames.Select(GeneratePackageAsync);
                var openSources = Task.WhenAll(genPackageTasks).Result.ToList();

                // nuget以外
                openSources.Add(
                    new OpenSource
                    {
                        Name = "FlatBuffers",
                        Auther = "Google",
                        Summry = "Memory Efficient Serialization Library",
                        Url = "https://github.com/google/flatbuffers"
                    });

                openSources.Add(
                    new OpenSource
                    {
                        Name = "LRU Cache",
                        Auther = "Yoshihiro Ito",
                        Summry = "Simple Implementation C# LRU Cache",
                        Url = "https://github.com/YoshihiroIto/Jewelry"
                    });

                using (var writer = new StringWriter())
                {
                    new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, openSources.OrderBy(x => x.Name));
                    return writer.ToString();
                }
            }
        }

        private static async Task<OpenSource> GeneratePackageAsync(string id)
        {
            var url = $"http://api-v2v3search-0.nuget.org/query?q={id}";

            using (var wc = new System.Net.WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                var jsonText = await wc.DownloadStringTaskAsync(url);
                var json = JsonConvert.DeserializeObject<Rootobject>(jsonText);

                var data = json.data.Single(x => x.id == id);

                return new OpenSource
                {
                    Name = data.title,
                    Auther = string.Join(" ", data.authors),
                    Summry = string.IsNullOrEmpty(data.summary) ? data.description : data.summary,
                    Url = data.projectUrl
                };
            }
        }

        // ReSharper disable InconsistentNaming
        public class Rootobject
        {
            public Context context { get; set; }
            public int totalHits { get; set; }
            public DateTime lastReopen { get; set; }
            public string index { get; set; }
            public Datum[] data { get; set; }
        }

        public class Context
        {
            public string vocab { get; set; }
            public string _base { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public string type { get; set; }
            public string registration { get; set; }
            //public string id { get; set; }
            public string version { get; set; }
            public string description { get; set; }
            public string summary { get; set; }
            public string title { get; set; }
            public string iconUrl { get; set; }
            public string licenseUrl { get; set; }
            public string projectUrl { get; set; }
            public string[] tags { get; set; }
            public string[] authors { get; set; }
            public int totalDownloads { get; set; }
            public Version[] versions { get; set; }
        }

        public class Version
        {
            public string version { get; set; }
            public int downloads { get; set; }
            public string id { get; set; }
        }

        // ReSharper restore InconsistentNaming
    }
}