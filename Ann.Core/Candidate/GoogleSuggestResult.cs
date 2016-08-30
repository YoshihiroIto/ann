using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Candidate
{
    public class GoogleSuggestResult : ICandidate
    {
        public GoogleSuggestResult(string result, LanguagesService languagesService)
        {
            _result = result;
            _languagesService = languagesService;
            _url = $"https://www.google.co.jp/search?q={WebUtility.UrlEncode(result)}";

            _RunCommand = new DelegateCommand(() =>
            {
                try
                {
                    Process.Start(_url);
                }
                catch
                {
                    // ignored
                }
            });
        }

        private readonly string _result;
        private readonly LanguagesService _languagesService;
        private readonly string _url;

        string ICandidate.Comment => _languagesService.GetString(StringTags.GoogleSuggest);
        Brush ICandidate.Icon => Application.Current?.Resources["IconGoogle"] as Brush;
        string ICandidate.Name => _result;
        MenuCommand[] ICandidate.SubCommands => null;
        bool ICandidate.CanSetPriority => false;

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;
    }
}