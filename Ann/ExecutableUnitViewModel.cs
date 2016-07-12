using System.Diagnostics;
using System.Windows.Media;
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

        public ImageSource Icon => _parent.GetIcon(Path);

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

        private readonly MainWindowViewModel _parent;

        public ExecutableUnitViewModel(MainWindowViewModel parent, ExecutableUnit model)
        {
            Debug.Assert(parent != null);
            Debug.Assert(model != null);

            _parent = parent;

            Name = string.IsNullOrWhiteSpace(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;
        }
    }
}