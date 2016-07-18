using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Media;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
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

        public ReactiveCommand FlipHighPriority { get; }

        private readonly MainWindowViewModel _parent;

        public ExecutableUnitViewModel(MainWindowViewModel parent, ExecutableUnit model)
        {
            Debug.Assert(parent != null);
            Debug.Assert(model != null);

            _parent = parent;

            Name = string.IsNullOrWhiteSpace(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;

            Observable.FromEventPattern(
                h => App.Instance.HighPriorityChanged += h,
                h => App.Instance.HighPriorityChanged -= h)
                .Subscribe(_ =>
                    // ReSharper disable once ExplicitCallerInfoArgument
                    RaisePropertyChanged(nameof(IsHighPriority))
                ).AddTo(CompositeDisposable);

            FlipHighPriority = new ReactiveCommand().AddTo(CompositeDisposable);
            FlipHighPriority
                .Subscribe(_ => IsHighPriority = !IsHighPriority)
                .AddTo(CompositeDisposable);
        }
    }
}