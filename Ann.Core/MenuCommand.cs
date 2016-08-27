using System.Windows.Input;

namespace Ann.Core
{
    public class MenuCommand
    {
        public StringTags Caption { get; set;  }
        public ICommand Command { get; set; }
    }
}