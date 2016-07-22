﻿using System;
using System.Diagnostics;
using System.Linq;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.PriorityFiles
{
    public class PriorityFilesViewModel : ViewModelBase
    {
        public string Name { get; } = "Priority Files";

        public ReadOnlyReactiveCollection<PathViewModel> Files { get; set; }

        public ReactiveCommand FileAddCommand { get; }
        public ReactiveCommand<PathViewModel> FileRemoveCommand { get; }

        public PriorityFilesViewModel(Config.App model)
        {
            Debug.Assert(model != null);

            var filters = new[]
            {
                new CommonFileDialogFilter("Executable file", "*.exe, *.lnk"), 
                new CommonFileDialogFilter("All file", "*.*") 
            };

            Files = model.PriorityFiles.ToReadOnlyReactiveCollection(p => new PathViewModel(p, Messenger, false, filters))
                .AddTo(CompositeDisposable);

            FileAddCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FileAddCommand.Subscribe(_ => model.PriorityFiles.Add(new Path(string.Empty)))
                .AddTo(CompositeDisposable);

            FileRemoveCommand = new ReactiveCommand<PathViewModel>().AddTo(CompositeDisposable);
            FileRemoveCommand.Subscribe(p =>
            {
                var t = model.PriorityFiles.FirstOrDefault(f => f.Value == p.Path.Value);
                if (t != null)
                    model.PriorityFiles.Remove(t);
            }).AddTo(CompositeDisposable);
        }
    }
}