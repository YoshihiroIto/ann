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
    public class Generator
    {
        public string Generate(IEnumerable<string> packagesConfigPaths)
        {
            using (new TimeMeasure())
            {
                var packageNames = packagesConfigPaths
                    .Select(XElement.Load)
                    .SelectMany(p => p.Elements("package"))
                    .Select(p => p.Attribute("id")?.Value)
                    .Where(x => x != null)
                    .Distinct();

                var genPackageTasks = packageNames.Select(GeneratePackageAsync);
                var openSources = Task.WhenAll(genPackageTasks).Result.ToList();

                using (var writer = new StringWriter())
                {
                    new Serializer(SerializationOptions.EmitDefaults).Serialize(writer,
                        openSources.Where(x => x != null).OrderBy(x => x.Name));
                    return writer.ToString();
                }
            }
        }

        private static async Task<OpenSource> GeneratePackageAsync(string id)
        {
            var url = $"http://api-v2v3search-0.nuget.org/query?q={id}";

            try
            {
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
            catch
            {
                return null;
            }
        }

        // ReSharper disable InconsistentNaming
        public class Rootobject
        {
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public string description { get; set; }
            public string summary { get; set; }
            public string title { get; set; }
            public string projectUrl { get; set; }
            public string[] tags { get; set; }
            public string[] authors { get; set; }
        }

        // ReSharper restore InconsistentNaming
    }
}