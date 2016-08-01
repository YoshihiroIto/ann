using System.Collections.ObjectModel;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Config
{
    public class MostRecentUsedList : ModelBase
    {
        #region PriorityFiles

        private ObservableCollection<string> _AppPath = new ObservableCollection<string>();

        public ObservableCollection<string> AppPath
        {
            get { return _AppPath; }
            set { SetProperty(ref _AppPath, value); }
        }

        #endregion
    }
}