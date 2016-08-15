using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Ann.Core;
using Ann.Foundation.Mvvm;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.PriorityFiles
{
    public class PriorityFilesViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<PathViewModel> Files { get; set; }

        public ReactiveCommand FileAddCommand { get; }
        public ReactiveCommand<PathViewModel> FileRemoveCommand { get; }

        public PriorityFilesViewModel(Core.Config.App model, App app)
        {
            Debug.Assert(model != null);

            Files = model.PriorityFiles.ToReadOnlyReactiveCollection(p =>
            {
                var isInitializing = true;
                using (Disposable.Create(() => isInitializing = false))
                {
                    var pvm = new PathViewModel(p, false,
                        () => new[]
                        {
                            new CommonFileDialogFilter(Properties.Resources.ExecutableFile,
                                string.Join(", ", model.ExecutableFileExts.Select(e => "*." + e))),
                            new CommonFileDialogFilter(Properties.Resources.AllFiles, "*.*")
                        });

                    pvm.Path
                        .Subscribe(_ =>
                        {
                            app.RefreshPriorityFiles();
                            app.InvokePriorityFilesChanged();
                        }).AddTo(pvm.CompositeDisposable);

                    // 未入力状態でフォーカスが外れたら削除する
                    pvm.IsFocused
                        .Where(i => i == false)
                        .Where(_ => isInitializing == false)
                        .Where(_ => string.IsNullOrEmpty(pvm.Path.Value))
                        .Subscribe(_ => model.PriorityFiles.Remove(p))
                        .AddTo(pvm.CompositeDisposable);

                    return pvm;
                }
            }).AddTo(CompositeDisposable);

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