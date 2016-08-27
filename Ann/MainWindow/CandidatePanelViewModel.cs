using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Core;
using Ann.Core.Candidate;
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

        public ICommand RunCommand => _model.RunCommand;
        public MenuCommand[] SubCommands => _model.SubCommands;

        public App App { get; }

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

                return App.IsPriorityFile(Comment);
            }

            set
            {
                if (value)
                {
                    if (App.AddPriorityFile(Comment))
                        RaisePropertyChanged();
                }
                else
                {
                    if (App.RemovePriorityFile(Comment))
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

        private readonly ICandidate _model;

        public CandidatePanelViewModel(ICandidate model, App app)
        {
            Debug.Assert(model != null);
            Debug.Assert(app != null);

            _model = model;
            App = app;
        }

        private bool _isSubscribedPriorityFilesChanged;

        private void SubscribPriorityFilesChanged()
        {
            Observable.FromEventPattern(
                h => App.PriorityFilesChanged += h,
                h => App.PriorityFilesChanged -= h)
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