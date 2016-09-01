using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Foundation
{
    public class GoogleSuggestService: DisposableNotificationObject
    {
        private readonly WebClient _webClient;

        public GoogleSuggestService()
        {
            _webClient = new WebClient().AddTo(CompositeDisposable);
        }

        public async Task<IEnumerable<string>> SuggestAsync(string input, string language)
        {
            try
            {
                var url = $"http://www.google.com/complete/search?hl={language}&output=toolbar&q={WebUtility.UrlEncode(input)}";

                var xml = await _webClient.DownloadStringTaskAsync(url);

                using (var sr = new StringReader(xml))
                {
                    return XDocument.Load(sr).Root?
                        .Descendants("suggestion")
                        .Select(x => x.Attribute("data")?.Value)
                        .Where(x => x != null);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
