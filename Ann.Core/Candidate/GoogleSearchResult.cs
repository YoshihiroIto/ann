using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Candidate
{
    public class GoogleSearchResult : ICandidate
    {
        public GoogleSearchResult(string input, LanguagesService languagesService, StringTags comment)
        {
            _Input = input;
            _languagesService = languagesService;
            _url = $"https://www.google.co.jp/search?q={WebUtility.UrlEncode(input)}";
            _comment = comment;

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

        private readonly string _Input;
        private readonly LanguagesService _languagesService;
        private readonly string _url;
        private readonly StringTags _comment;

        string ICandidate.Comment => _languagesService.GetString(_comment);
        Brush ICandidate.Icon => Application.Current?.Resources["IconGoogle"] as Brush;
        string ICandidate.Name => _Input;
        MenuCommand[] ICandidate.SubCommands => null;
        bool ICandidate.CanSetPriority => false;

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;
    }
}