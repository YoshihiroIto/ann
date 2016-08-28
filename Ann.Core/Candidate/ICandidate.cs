using System.Windows.Input;
using System.Windows.Media;

namespace Ann.Core.Candidate
{
    public interface ICandidate
    {
        Brush Icon { get; }
        string Name { get; }
        string Comment { get; }
       
        ICommand RunCommand { get; }
        MenuCommand[] SubCommands { get; }

        bool CanSetPriority { get; }
    }
}