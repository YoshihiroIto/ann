using System;
using System.Diagnostics;
using System.Linq;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage.About;
using Ann.SettingWindow.SettingPage.General;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann.SettingWindow
{
    public class SettingViewModel : ViewModelBase
    {
        public ViewModelBase[] Pages { get; }
        public ReactiveProperty<ViewModelBase> SelectedPage { get; }

        public ReactiveCommand InitializeCommand { get; }
        public ReactiveCommand CloseCommand { get; }

        public SettingViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            CompositeDisposable.Add(() => Pages.ForEach(p => p.Dispose()));

            InitializeCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            InitializeCommand.Subscribe(async _ => await _about.CheckVersion()).AddTo(CompositeDisposable);

            CloseCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            CloseCommand
                .Subscribe(_ => MessageBroker.Default.Publish(new WindowActionMessage {Action = WindowAction.Close}))
                .AddTo(CompositeDisposable);

            Pages = new ViewModelBase[]
            {
                new GeneralViewModel(model),
                new ShortcutsViewModel(model),
                new TargetFoldersViewModel(model),
                new PriorityFilesViewModel(model),
                _about = new AboutViewModel() 
            };
            SelectedPage = new ReactiveProperty<ViewModelBase>(Pages[0]).AddTo(CompositeDisposable);
        }

        private readonly AboutViewModel _about;
    }
}