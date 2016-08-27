using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ann.Core.Candidate
{
    public class CalculationResult : ICandidate
    {
        public CalculationResult(string result)
        {
            _result = result;
        }

        private readonly string _result;

        string ICandidate.Comment => "Calculator";
        Brush ICandidate.Icon => Application.Current.Resources["IconCalculator"] as Brush;
        string ICandidate.Name => _result;
        ICommand ICandidate.RunCommand => null;
        MenuCommand[] ICandidate.SubCommands => null;
        bool ICandidate.CanSetPriority => false;
    }
}