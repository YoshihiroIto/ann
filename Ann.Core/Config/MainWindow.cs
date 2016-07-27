using Ann.Foundation.Mvvm;

namespace Ann.Core.Config
{
    public class MainWindow : ModelBase
    {
        #region Left

        private double _Left = double.NaN;

        public double Left
        {
            get { return _Left; }
            set { SetProperty(ref _Left, value); }
        }

        #endregion

        #region Top

        private double _Top = double.NaN;

        public double Top
        {
            get { return _Top; }
            set { SetProperty(ref _Top, value); }
        }

        #endregion
    }
}