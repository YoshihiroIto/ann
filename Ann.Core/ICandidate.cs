using System.Windows.Media;

namespace Ann.Core
{
    public interface ICandidate
    {
        string Name { get; }
        string Comment { get; }

        ImageSource Icon { get; }
    }
}