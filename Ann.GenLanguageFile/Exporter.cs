using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using CsvHelper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Ann.GenLanguageFile
{
    public class Exporter
    {
        public class OutputOptions
        {
            public string Namespace { get; set; }
        }

        public class Result
        {
            public string Class { get; set; }
            public string DefaultXaml { get; set; }
        }

        public async Task<Result> Export(OutputOptions options)
        {
            var scopes = new[] {DriveService.Scope.DriveReadonly};
            var homePath = Environment.OSVersion.Platform == PlatformID.Unix ||
                           Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            var clientSecretFilePath =
                Environment.ExpandEnvironmentVariables($@"{homePath}\client_secret_Ann-Localization.json");

            UserCredential credential;
            using (var stream = new FileStream(clientSecretFilePath, FileMode.Open, FileAccess.Read))
            {
                var credPath =
                    Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    ".credentials/Ann-Localization.json");

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            string csvString;
            using (var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Ann-Localization"
            }))
            {
                var exportResult = service.Files.Export("15Eea4QU9iY2JBkNnNlo83EbCq4QeIJVOnnnKh-c93xs", "text/csv");

                using (var ms = new MemoryStream())
                {
                    await exportResult.DownloadAsync(ms);

                    csvString = Encoding.UTF8.GetString(ms.ToArray());
                }
            }

            return ExportInternal(csvString, options);
        }

        private Result ExportInternal(string csvString, OutputOptions options)
        {
            string[] languages = null;
            var languageData = new List<LanguageData>();

            using (var sr = new StringReader(csvString))
            {
                var parser = new CsvParser(sr);

                while(true)
                {
                    var row = parser.Read();
                    if (row == null)
                        break;

                    if (languages == null)
                        languages = row.Skip(1).ToArray();
                    else
                        languageData.Add(new LanguageData
                        {
                            Tag = row[0],
                            Data = row.Skip(1).ToArray()
                        });
                }
            }

            if (languages != null)
                return new Result
                {
                    Class = ExportClass(languages, languageData, options),
                    DefaultXaml = ExportDefaultXaml(languageData)
                };

            return null;
        }

        private static string ExportClass(string[] languages, List<LanguageData> languageData, OutputOptions options)
        {
            var langCase = new StringBuilder();
            languages.ForEach((language, i) =>
            {
                var tagCase = new StringBuilder();
                languageData.ForEach(data =>
                {
                    tagCase.Append(
                        string.Format(
                            Properties.Resources.CaseNoReturn,
                            "StringTags",
                            data.Tag,
                            string.Format(Properties.Resources.TagCaseReturn, data.Data[i])));
                });

                var tagSwitch = string.Format(Properties.Resources.Switch, "tag", tagCase);

                langCase.Append(
                    string.Format(
                        Properties.Resources.Case,
                        "Languages",
                        language,
                        tagSwitch));
            });

            var switches = string.Format(Properties.Resources.Switch, "language", langCase);

            var fileImage =
                string.Format(Properties.Resources.File,
                    options.Namespace,
                    string.Format(Properties.Resources.Languages, string.Join(",\r\n", languages)),
                    string.Format(Properties.Resources.Tags, string.Join(",\r\n", languageData.Select(l => l.Tag))),
                    string.Format(Properties.Resources.LocalizationClass, switches));

            return fileImage;
        }

        private static string ExportDefaultXaml(List<LanguageData> languageData)
        {
            var resDict = new ResourceDictionary();

            foreach (var l in languageData)
                resDict.Add(l.Tag, l.Data[0]);

            return  XamlWriter.Save(resDict); 
        }

        private class LanguageData
        {
            public string Tag { get; set; }
            public string[] Data { get; set; }
        }
    }
}