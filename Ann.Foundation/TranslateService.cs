﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ann.Foundation.Mvvm;
using Newtonsoft.Json;
using Reactive.Bindings.Extensions;

namespace Ann.Foundation
{
    // http://matatabi-ux.hateblo.jp/entry/2015/08/31/120000 を元にしました
    public class TranslateService : DisposableNotificationObject
    {
        private readonly string _ClientId;
        private readonly string _ClientSecret;

        private DateTime _AccessTokenExpires = DateTime.MinValue;

        private static readonly string OAuthUri =
            "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

        private static readonly string TranslateUriFromTo =
            "http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}";

        private static readonly string TranslateUriTo =
            "http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&to={1}";

        private string _AccessToken = string.Empty;

        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cts;

        private bool _isDownloading;

        public TranslateService(string clientId, string clientSecret)
        {
            _ClientId = clientId;
            _ClientSecret = clientSecret;

            _httpClient = new HttpClient().AddTo(CompositeDisposable);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            _cts = new CancellationTokenSource().AddTo(CompositeDisposable);
        }

        public void CancelTranslate()
        {
            if (_isDownloading == false)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        public async Task<bool> AuthenticateAsync()
        {
            using (Disposable.Create(() => _isDownloading = false))
            {
                try
                {
                    _isDownloading = true;

                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"client_id", _ClientId},
                        {"client_secret", _ClientSecret},
                        {"grant_type", "client_credentials"},
                        {"scope", "http://api.microsofttranslator.com"},
                    });

                    var result = await _httpClient.PostAsync(OAuthUri, content, _cts.Token);
                    if (result.IsSuccessStatusCode == false)
                        return false;

                    var json = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JContainer>(json);
                    var now = DateTime.Now;

                    _AccessToken = response.Value<string>("access_token");
                    var expiresIn = response.Value<long>("expires_in");
                    _AccessTokenExpires = now.AddSeconds(expiresIn);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsExpierd => string.IsNullOrEmpty(_AccessToken) || _AccessTokenExpires.CompareTo(DateTime.Now) < 0;

        public async Task<string> TranslateAsync(string input, LanguageCodes from, LanguageCodes to)
        {
            if (string.IsNullOrEmpty(_ClientId) || string.IsNullOrEmpty(_ClientSecret))
                return null;

            try
            {
                if (IsExpierd)
                {
                    var i = await AuthenticateAsync();
                    if (i == false)
                        return null;
                }

                string translated;
                {
                    using (Disposable.Create(() => _isDownloading = false))
                    {
                        _isDownloading = true;

                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_AccessToken}");

                        var url = @from == LanguageCodes.AutoDetect
                            ? string.Format(TranslateUriTo, WebUtility.UrlEncode(input), ToString(to))
                            : string.Format(TranslateUriFromTo, WebUtility.UrlEncode(input), ToString(@from),
                                ToString(to));

                        string xml;
                        using (var response = await _httpClient.GetAsync(url, _cts.Token))
                            xml = await response.Content.ReadAsStringAsync();

                        using (var sr = new StringReader(xml))
                        {
                            var xdoc = XDocument.Load(sr);
                            translated = xdoc.Root?.Value;
                        }
                    }
                }

                return translated;
            }
            catch
            {
                return null;
            }
        }

        private static string ToString(LanguageCodes c)
        {
            return c.ToString().Replace("_", "-");
        }

        public enum LanguageCodes
        {
            // ReSharper disable InconsistentNaming

            AutoDetect,
            //
            af, // Afrikaans
            ar, // Arabic
            bs_Latn, // Bosnian (Latin)
            bg, // Bulgarian
            ca, // Catalan
            zh_CHS, // Chinese Simplified
            zh_CHT, // Chinese Traditional
            hr, // Croatian
            cs, // Czech
            da, // Danish
            nl, // Dutch
            en, // English
            et, // Estonian
            fi, // Finnish
            fr, // French
            de, // German
            el, // Greek
            ht, // Haitian Creole
            he, // Hebrew
            hi, // Hindi
            mww, // Hmong Daw
            hu, // Hungarian
            id, // Indonesian
            it, // Italian
            ja, // Japanese
            sw, // Kiswahili
            tlh, // Klingon
            tlh_Qaak, // Klingon (pIqaD)
            ko, // Korean
            lv, // Latvian
            lt, // Lithuanian
            ms, // Malay
            mt, // Maltese
            no, // Norwegian
            fa, // Persian
            pl, // Polish
            pt, // Portuguese
            otq, // Querétaro Otomi
            ro, // Romanian
            ru, // Russian
            sr_Cyrl, // Serbian (Cyrillic)
            sr_Latn, // Serbian (Latin)
            sk, // Slovak
            sl, // Slovenian
            es, // Spanish
            sv, // Swedish
            th, // Thai
            tr, // Turkish
            uk, // Ukrainian
            ur, // Urdu
            vi, // Vietnamese
            cy, // Welsh
            yua // Yucatec Maya

            // ReSharper restore InconsistentNaming
        }
    }
}