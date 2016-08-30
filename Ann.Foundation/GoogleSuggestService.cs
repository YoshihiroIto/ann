using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ann.Foundation
{
    public static class GoogleSuggestService
    {
        public static async Task<string[]> SuggestAsync(string input, string language)
        {
            try
            {
                var url = $"http://www.google.com/complete/search?hl={language}&output=toolbar&q={WebUtility.UrlEncode(input)}";

                using (var wc = new WebClient())
                {
                    var xml = await wc.DownloadStringTaskAsync(url);

                    using (var sr = new StringReader(xml))
                    {
                        return XDocument.Load(sr).Root?
                            .Descendants("suggestion")
                            .Select(x => x.Attribute("data")?.Value)
                            .Where(x => x != null)
                            .ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
