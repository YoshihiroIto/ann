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

                return _app.IsPriorityFile(Path);
            }

            set
            {
                if (value)
                {
                    if (_app.AddPriorityFile(Path))
                        RaisePropertyChanged();
                }
                else
                {
                    if (_app.RemovePriorityFile(Path))
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
        private readonly App _app;

        public ExecutableUnitViewModel(MainWindowViewModel parent, ExecutableUnit model, App app)
        {
            Debug.Assert(parent != null);
            Debug.Assert(model != null);
            Debug.Assert(app != null);

            _parent = parent;
            _app = app;

            Name = string.IsNullOrWhiteSpace(model.Name) == false ? model.Name : System.IO.Path.GetFileName(model.Path);
            Path = model.Path;
        }

        private bool _isSubscribedPriorityFilesChanged;

        private void SubscribPriorityFilesChanged()
        {
            Observable.FromEventPattern(
                h => _app.PriorityFilesChanged += h,
                h => _app.PriorityFilesChanged -= h)
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