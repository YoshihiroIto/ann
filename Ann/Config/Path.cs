using Ann.Foundation.Mvvm;
using Jil;

namespace Ann.Config
{
    public class Path : ModelBase
    {
        #region Value

        private string _Value = string.Empty;

        [JilDirective("Path")]
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