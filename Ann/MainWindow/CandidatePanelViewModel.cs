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
        public Brush Icon => _model.Icon;

        public ICommand RunCommand => _model.RunCommand;
        public MenuCommand[] SubCommands => _model.SubCommands;

        public bool CanSetPriority => _model.CanSetPriority;

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
                if (_model.CanSetPriority == false)
                    return false;

                if (_isSubscribedPriorityFilesChanged == false)
                {
                    SubscribPriorityFilesChanged();
                    _isSubscribedPriorityFilesChanged = true;
                }

                return App.IsPriorityFile(Comment);
            }

            set
            {
                if (_model.CanSetPriority == false)
                    return;

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

        public CandidatePanelViewModel(ICandidate model, App app, Core.Config.App config)
        {
            Debug.Assert(model != null);
            Debug.Assert(app != null);
            Debug.Assert(config != null);

            _model = model;
            App = app;

            config.ObserveProperty(c => c.Culture)
                // ReSharper disable once ExplicitCallerInfoArgument
                .Subscribe(_ => RaisePropertyChanged(nameof(Comment)))
                .AddTo(CompositeDisposable);
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