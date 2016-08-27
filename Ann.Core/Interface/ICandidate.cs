using System.Windows.Input;
using System.Windows.Media;

namespace Ann.Core.Interface
{
    public interface ICandidate
    {
        ImageSource Icon { get; }
        string Name { get; }
        string Comment { get; }
       
        ICommand RunCommand { get; }
        MenuCommand[] SubCommands { get; }
    }
}