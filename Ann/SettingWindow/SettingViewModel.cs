using System;
using System.Diagnostics;
using System.Linq;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow.SettingPage.About;
using Ann.SettingWindow.SettingPage.Functions;
using Ann.SettingWindow.SettingPage.General;
using Ann.SettingWindow.SettingPage.PriorityFiles;
using Ann.SettingWindow.SettingPage.Shortcuts;
using Ann.SettingWindow.SettingPage.TargetFolders;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow
{
    public class SettingViewModel : ViewModelBase
    {
        public ViewModelBase[] Pages { get; }
        public ReactiveProperty<ViewModelBase> SelectedPage { get; }

        public ReactiveCommand InitializeCommand { get; }
        public ReactiveCommand CloseCommand { get; }

        public WindowMessageBroker Messenger { get; }

        public App App { get; }

        public SettingViewModel(Core.Config.App model, VersionUpdater versionUpdater, App app)
        {
            Debug.Assert(model != null);
            Debug.Assert(app != null);

            App = app;

            CompositeDisposable.Add(() => Pages.ForEach(p => p.Dispose()));

            Messenger = new WindowMessageBroker().AddTo(CompositeDisposable);

            InitializeCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            InitializeCommand
                .Subscribe(async _ => await versionUpdater.CheckAsync())
                .AddTo(CompositeDisposable);

            CloseCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            CloseCommand
                .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.Close)))
                .AddTo(CompositeDisposable);

            Pages = new ViewModelBase[]
            {
                new GeneralViewModel(model, versionUpdater),
                new ShortcutsViewModel(model),
                new TargetFoldersViewModel(model, app, Messenger),
                new PriorityFilesViewModel(model, app, Messenger),
                new FunctionsViewModel(model), 
                new AboutViewModel(versionUpdater, Messenger)
            };
            SelectedPage = new ReactiveProperty<ViewModelBase>(Pages[0]).AddTo(CompositeDisposable);
        }
    }
}