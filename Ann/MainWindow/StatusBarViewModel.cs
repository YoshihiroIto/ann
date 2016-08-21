using System;
using System.Diagnostics;
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

        private readonly App _app;

        public StatusBarViewModel(App app)
        {
            Debug.Assert(app != null);

            _app = app;

            Messages = new ReactiveCollection<StatusBarItemViewModel>().AddTo(CompositeDisposable);

            CompositeDisposable.Add(async () => await app.CancelUpdateIndexAsync());
            CompositeDisposable.Add(() => Messages.ForEach(x => x.Dispose()));

            Visibility = Messages.CollectionChangedAsObservable()
                .Select(_ => Messages.Any() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed)
                .ToReadOnlyReactiveProperty(System.Windows.Visibility.Collapsed)
                .AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsIndexUpdating)
                .SubscribeOn(ReactivePropertyScheduler.Default)
                .Subscribe(i =>
                {
                    if (i)
                    {
                        var item = new ProcessingStatusBarItemViewModel(
                            app,
                            StatusBarItemViewModel.SearchKey.IndexUpdating,
                            StringTags.Message_IndexUpdating);
                        Messages.Add(item);
                    }
                    else
                    {
                        var item = Messages
                            .FirstOrDefault(
                                x => x.Key == StatusBarItemViewModel.SearchKey.IndexUpdating);

                        if (item == null)
                            return;

                        Messages.Remove(item);
                        item.Dispose();
                    }
                }).AddTo(CompositeDisposable);

            app.ObserveProperty(c => c.Crawling)
                .SubscribeOn(ReactivePropertyScheduler.Default)
                .Subscribe(c =>
                {
                    var item = Messages
                        .FirstOrDefault(
                            x => x.Key == StatusBarItemViewModel.SearchKey.IndexUpdating);

                    if (item != null)
                        item.Messages.Value =
                            new[]
                            {
                                new StatusBarItemViewModel.Message
                                {
                                    String = StringTags.Message_IndexUpdating,
                                    Options = new object[] {c.ToString()}
                                }
                            };
                })
                .AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsEnableActivateHotKey)
                .Subscribe(i =>
                {
                    if (i == false)
                    {
                        var item = new StatusBarItemViewModel(
                            app,
                            StatusBarItemViewModel.SearchKey.ActivationShortcutKeyIsAlreadyInUse,
                            StringTags.Message_ActivationShortcutKeyIsAlreadyInUse);
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

            app.ObserveProperty(x => x.IndexOpeningResult)
                .Subscribe(r =>
                {
                    if (r == IndexOpeningResults.InOpening)
                    {
                        var item = new ProcessingStatusBarItemViewModel(
                            app,
                            StatusBarItemViewModel.SearchKey.InOpening,
                            StringTags.Message_InOpening);
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

            SetupVersionUpdater(app);
        }

        private StatusBarItemViewModel _autoUpdaterItem;

        private void SetupVersionUpdater(App app)
        {
            CompositeDisposable.Add(() => _autoUpdaterItem?.Dispose());

            _app.ObserveProperty(x => x.AutoUpdateState)
                .Subscribe(s =>
                {
                    if (_autoUpdaterItem != null)
                    {
                        Messages.RemoveOnScheduler(_autoUpdaterItem);
                        _autoUpdaterItem.Dispose();
                    }

                    if (s == App.AutoUpdateStates.CloseAfterNSec)
                    {
                        _autoUpdaterItem =
                            new WaitingStatusBarItemViewModel(
                                app,
                                StringTags.AutoUpdateStates_CloseAfterNSec,
                                new object[] {Constants.AutoUpdateCloseDelaySec.ToString()});
                    }

                    if (_autoUpdaterItem != null)
                        Messages.Add(_autoUpdaterItem);
                }).AddTo(CompositeDisposable);

            _app.ObserveProperty(x => x.AutoUpdateRemainingSeconds)
                .Subscribe(p =>
                {
                    if (_app.AutoUpdateState == App.AutoUpdateStates.CloseAfterNSec)
                    {
                        _autoUpdaterItem.Messages.Value =
                            p == 0
                                ? new[]
                                {
                                    new StatusBarItemViewModel.Message
                                    {
                                        String = StringTags.AutoUpdateStates_CloseAfter0Sec_Restart,
                                    }
                                }
                                : new[]
                                {
                                    new StatusBarItemViewModel.Message
                                    {
                                        String = StringTags.AutoUpdateStates_CloseAfter0Sec_Restart,
                                        Options = new object[] {p.ToString()}
                                    }
                                };
                    }
                }).AddTo(CompositeDisposable);
        }
    }
}