﻿using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Shortcuts
{
    public class ShortcutsViewModel : ViewModelBase
    {
        public string Name { get; } = "Shortcuts";
        public bool IsAbout { get; } = false;

        public ShortcutKeyViewModel Activate { get; set; }
        public ShortcutKeyListBoxViewModel HideShortcuts { get; }

        public ShortcutsViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            Activate = new ShortcutKeyViewModel(model.MainWindow.ShortcutKeys.Activate)
                .AddTo(CompositeDisposable);

            HideShortcuts = new ShortcutKeyListBoxViewModel(model.MainWindow.ShortcutKeys.Hide)
                .AddTo(CompositeDisposable);
        }
    }
}