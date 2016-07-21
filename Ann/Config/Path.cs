using Ann.Foundation.Mvvm;
using YamlDotNet.Serialization;

namespace Ann.Config
{
    public class Path : ModelBase
    {
        #region Value

        private string _Value = string.Empty;

        [YamlMember(Alias = "Path")]
        public string Value
        {
            get { return _Value; }
            set { SetProperty(ref _Value, value); }
        }

        #endregion

        public Path()
        {
        }

        public Path(string p)
        {
            Value = p;
        }
    }
}