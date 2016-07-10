using System.Diagnostics;
using Ann.Core;
using Ann.Foundation.Mvvm;

namespace Ann
{
    public class ExecutableUnitViewModel : ViewModelBase
    {
        #region Name

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        #endregion

        #region Path

        private string _Path;

        public string Path
        {
            get { return _Path; }
            set { SetProperty(ref _Path, value); }
        }

        #endregion

        public bool IsHighPriority
        {
            get { return App.Instance.IsHighPriority(Path); }
            set
            {
                if (value)
                {
                   if (App.Instance.AddHighPriorityPath(Path))
                        RaisePropertyChanged();
                }
                else
                {
                   if (App.Instance.RemoveHighPriorityPath(Path))
                        RaisePropertyChanged();
                }
            }
        }

        public ExecutableUnitViewModel(ExecutableUnit model)
        {
            Debug.Assert(model != null);

            Name = string.IsNullOrEmpty(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;
        }
    }
}