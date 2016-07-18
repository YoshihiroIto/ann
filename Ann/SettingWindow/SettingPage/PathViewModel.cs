﻿using System;
using System.Diagnostics;
using Ann.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }

        public ReactiveCommand FolderSelectDialogOpenCommand { get; }
        
        public PathViewModel(Path model)
        {
            Debug.Assert(model != null);
            
            Path = model.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(CompositeDisposable);

            FolderSelectDialogOpenCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            FolderSelectDialogOpenCommand.Subscribe(_ =>
            {
                // todo:viewを操作している
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;

                    var r = dialog.ShowDialog();
                    if (r == CommonFileDialogResult.Ok)
                        Path.Value = dialog.FileName;
                }
            }).AddTo(CompositeDisposable);
        }
    }
}