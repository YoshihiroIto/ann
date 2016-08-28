using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Candidate
{
    public class TranslateResult : ICandidate
    {
        public TranslateResult(string result)
        {
            _result = result;

            _RunCommand = new DelegateCommand(() => Clipboard.SetText(result));
        }

        private readonly string _result;

        string ICandidate.Comment => "Translator";
        Brush ICandidate.Icon => Application.Current?.Resources["IconTranslator"] as Brush;
        string ICandidate.Name => _result;
        MenuCommand[] ICandidate.SubCommands => null;
        bool ICandidate.CanSetPriority => false;

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;
    }
}