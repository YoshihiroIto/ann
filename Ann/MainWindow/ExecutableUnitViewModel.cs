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

        public bool IsPriorityFile
        {
            get
            {
                if (_isSubscribedPriorityFilesChanged == false)
                {
                    SubscribPriorityFilesChanged();
                    _isSubscribedPriorityFilesChanged = true;
                }

                return App.Instance.IsPriorityFile(Path);
            }

            set
            {
                if (value)
                {
                    if (App.Instance.AddPriorityFile(Path))
                        RaisePropertyChanged();
                }
                else
                {
                    if (App.Instance.RemovePriorityFile(Path))
                        RaisePropertyChanged();
                }
            }
        }

        private ReactiveCommand _IsPriorityFileFlipCommand;

        public ReactiveCommand IsPriorityFileFlipCommand
        {
            get
            {
                if (_IsPriorityFileFlipCommand == null)
                    SetupCommand();

                return _IsPriorityFileFlipCommand;
            }
        }

        private readonly MainWindowViewModel _parent;

        public ExecutableUnitViewModel(MainWindowViewModel parent, ExecutableUnit model)
        {
            Debug.Assert(parent != null);

            _parent = parent;

            Name = string.IsNullOrWhiteSpace(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;

        }

        private bool _isSubscribedPriorityFilesChanged;

        private void SubscribPriorityFilesChanged()
        {
            Observable.FromEventPattern(
                h => App.Instance.PriorityFilesChanged += h,
                h => App.Instance.PriorityFilesChanged -= h)
                .Subscribe(_ =>
                    // ReSharper disable once ExplicitCallerInfoArgument
                    RaisePropertyChanged(nameof(IsPriorityFile))
                ).AddTo(CompositeDisposable);
        }

        private void SetupCommand()
        {
            _IsPriorityFileFlipCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            _IsPriorityFileFlipCommand
                .Subscribe(_ => IsPriorityFile = !IsPriorityFile)
                .AddTo(CompositeDisposable);
        }
    }
}