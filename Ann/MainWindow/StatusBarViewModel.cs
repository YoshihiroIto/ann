using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
{
    public class StatusBarViewModel : ViewModelBase
    {
        public ReactiveCollection<StatusBarItemViewModel> Messages { get; }
        public ReadOnlyReactiveProperty<Visibility> Visibility { get; }

        public StatusBarViewModel(MainWindowViewModel parent)
        {
            Messages = new ReactiveCollection<StatusBarItemViewModel>().AddTo(CompositeDisposable);
            CompositeDisposable.Add(() => Messages.ForEach(x => x.Dispose()));

            Visibility = Messages.CollectionChangedAsObservable()
                .Select(_ => Messages.Any() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            parent.IsIndexUpdating.Subscribe(i =>
            {
                if (i)
                {
                    var item = new ProcessingStatusBarItemViewModel(
                        StatusBarItemViewModel.SearchKey.IndexUpdating,
                        Properties.Resources.Message_IndexUpdating);
                    Messages.AddOnScheduler(item);
                }
                else
                {
                    var item = Messages
                        .FirstOrDefault(
                            x => x.Key == StatusBarItemViewModel.SearchKey.IndexUpdating);

                    Messages.RemoveOnScheduler(item);
                    item?.Dispose();
                }
            }).AddTo(CompositeDisposable);


            parent.IsEnableActivateHotKey
                .Subscribe(i =>
                {
                    if (i == false)
                    {
                        var item = new StatusBarItemViewModel(
                            StatusBarItemViewModel.SearchKey.NoKey,
                            Properties.Resources.Message_ActivationShortcutKeyIsAlreadyInUse);
                        Messages.AddOnScheduler(item);
                    }
                    else
                    {
                        var item = Messages
                            .FirstOrDefault(
                                x => x.Key == StatusBarItemViewModel.SearchKey.ActivationShortcutKeyIsAlreadyInUse);

                        Messages.RemoveOnScheduler(item);
                        item?.Dispose();
                    }
                }).AddTo(CompositeDisposable);

            parent.IndexOpeningResult
                .Subscribe(r =>
                {
                    if (r == IndexOpeningResults.InOpening)
                    {
                        var item = new ProcessingStatusBarItemViewModel(
                            StatusBarItemViewModel.SearchKey.InOpening,
                            Properties.Resources.Message_InOpening);
                        Messages.AddOnScheduler(item);
                    }
                    else
                    {
                        var item = Messages
                            .FirstOrDefault(
                                x => x.Key == StatusBarItemViewModel.SearchKey.InOpening);

                        Messages.RemoveOnScheduler(item);
                        item?.Dispose();
                    }
                }).AddTo(CompositeDisposable);
        }
    }
}