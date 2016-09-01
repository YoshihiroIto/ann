using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.Foundation
{
    public class GoogleSuggestService: DisposableNotificationObject
    {
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cts;

        private bool _isDownloading;

        public GoogleSuggestService()
        {
            _httpClient = new HttpClient().AddTo(CompositeDisposable);
            _cts = new CancellationTokenSource();
        }

        public void CancelSuggest()
        {
            if (_isDownloading)
                _cts.Cancel();
        }

        public async Task<IEnumerable<string>> SuggestAsync(string input, string language)
        {
            try
            {
                string xml;
                {
                    using (Disposable.Create(() => _isDownloading = false))
                    {
                        var url = $"http://www.google.com/complete/search?hl={language}&output=toolbar&q={WebUtility.UrlEncode(input)}";

                        _isDownloading = true;

                        using (var response = await _httpClient.GetAsync(url, _cts.Token))
                            xml = await response.Content.ReadAsStringAsync();
                    }
                }

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
                _cts.Dispose();
                _cts = new CancellationTokenSource();

                return Enumerable.Empty<string>();
            }
        }
    }
}
