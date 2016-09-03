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
        public string Name => Model.Name;
        public string Comment => Model.Comment;
        public Brush Icon => Model.Icon;

        public ICommand RunCommand => Model.RunCommand;
        public MenuCommand[] SubCommands => Model.SubCommands;

        public bool CanSetPriority => Model.CanSetPriority;

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
                if (Model.CanSetPriority == false)
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
                if (Model.CanSetPriority == false)
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

        public ICandidate Model { get; set; }

        public SelectedBehavior SelectedBehavior => Model.SelectedBehavior;
        public string CommandWord => Model.CommandWord;
        public string InputWord => Model.Name;

        public CandidatePanelViewModel(App app, Core.Config.App config)
        {
            Debug.Assert(app != null);
            Debug.Assert(config != null);

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