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
    public class CandidatePanelViewModel : ViewModelBase
    {
        public string Name => _model.Name;
        public string Comment => _model.Comment;
        public ImageSource Icon => _model.Icon;

#region IsSelected

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

#endregion

        public bool IsPriorityFile
        {
            get
            {
                if (_isSubscribedPriorityFilesChanged == false)
                {
                    SubscribPriorityFilesChanged();
                    _isSubscribedPriorityFilesChanged = true;
                }

                return _app.IsPriorityFile(Comment);
            }

            set
            {
                if (value)
                {
                    if (_app.AddPriorityFile(Comment))
                        RaisePropertyChanged();
                }
                else
                {
                    if (_app.RemovePriorityFile(Comment))
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

        public ReactiveCommand<string> RunCommand => _parent.RunCommand;
        public ReactiveCommand ContainingFolderOpenCommand => _parent.ContainingFolderOpenCommand;

        private readonly MainWindowViewModel _parent;
        private readonly ICandidate _model;
        private readonly App _app;

        public CandidatePanelViewModel(MainWindowViewModel parent, ICandidate model, App app)
        {
            Debug.Assert(parent != null);
            Debug.Assert(model != null);
            Debug.Assert(app != null);

            _parent = parent;
            _model = model;
            _app = app;
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