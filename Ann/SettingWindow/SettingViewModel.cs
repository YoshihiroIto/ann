using System;
using System.Diagnostics;
using System.Linq;
using Ann.Foundation.Mvvm;
using Ann.SettingWindow.SettingPage.About;
using Ann.SettingWindow.SettingPage.General;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Livet;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow
{
    public class SettingViewModel : ViewModel
    {
        public ViewModelBase[] Pages { get; }
        public ReactiveProperty<ViewModelBase> SelectedPage { get; }

        public ReactiveCommand CloseCommand { get; }

        public SettingViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            CompositeDisposable.Add(() => Pages.ForEach(p => p.Dispose()));

            CloseCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            CloseCommand
                .Subscribe(_ => Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction")))
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

        public async void Initialize()
        {
            await _about.CheckVersion();
        }
    }
}