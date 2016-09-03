using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Splat;

namespace Ann.MainWindow
{
    public class StatusBarViewModel : ViewModelBase
    {
        public ObservableCollection<StatusBarItemViewModel> Messages { get; }
        public ReadOnlyReactiveProperty<Visibility> Visibility { get; }

        private readonly object _messageRemoveLock = new object();

        public StatusBarViewModel(App app)
        {
            Debug.Assert(app != null);

            Messages = new ObservableCollection<StatusBarItemViewModel>();

            if (ModeDetector.InUnitTestRunner() == false)
                BindingOperations.EnableCollectionSynchronization(Messages, new object());

            CompositeDisposable.Add(async () => await app.CancelUpdateIndexAsync());
            CompositeDisposable.Add(() =>
            {
                lock (_messageRemoveLock)
                {
                    Messages.ForEach(x => x.Dispose());
                    Messages.Clear();
                }
            });

            Visibility = Messages.CollectionChangedAsObservable()
                .Select(_ => Messages.Any()
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed)
                .ToReadOnlyReactiveProperty(System.Windows.Visibility.Collapsed)
                .AddTo(CompositeDisposable);

            Observable.FromEventPattern<
                    EventHandler<App.NotificationEventArgs>,
                    App.NotificationEventArgs>(
                    h => app.Notification += h,
                    h => app.Notification -= h)
                .Subscribe(async e =>
                {
                    var item = new StatusBarItemViewModel(app, e.EventArgs.Messages);

                    Messages.Add(item);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    Messages.Remove(item);
                    item.Dispose();
                })
                .AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsIndexUpdating)
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
                        lock (_messageRemoveLock)
                        {
                            var item = Messages
                                .FirstOrDefault(
                                    x => x.Key == StatusBarItemViewModel.SearchKey.IndexUpdating);

                            if (item == null)
                                return;

                            Messages.Remove(item);
                            item.Dispose();
                        }
                    }
                }).AddTo(CompositeDisposable);

            app.ObserveProperty(c => c.CrawlingCount)
                .Subscribe(c =>
                {
                    lock (_messageRemoveLock)
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
                                        Options = new object[] {c}
                                    }
                                };
                    }
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
                        Messages.Add(item);
                    }
                    else
                    {
                        lock (_messageRemoveLock)
                        {
                            var item = Messages
                                .FirstOrDefault(
                                    x => x.Key == StatusBarItemViewModel.SearchKey.ActivationShortcutKeyIsAlreadyInUse);

                            Messages.Remove(item);
                            item?.Dispose();
                        }
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
                        Messages.Add(item);
                    }
                    else
                    {
                        lock (_messageRemoveLock)
                        {
                            var item = Messages
                                .FirstOrDefault(
                                    x => x.Key == StatusBarItemViewModel.SearchKey.InOpening);

                            Messages.Remove(item);
                            item?.Dispose();
                        }
                    }
                }).AddTo(CompositeDisposable);

            app.ObserveProperty(c => c.IndexOpeningProgress)
                .Subscribe(c =>
                {
                    lock (_messageRemoveLock)
                    {
                        var item = Messages
                            .FirstOrDefault(
                                x => x.Key == StatusBarItemViewModel.SearchKey.InOpening);

                        if (item != null)
                            item.Messages.Value =
                                new[]
                                {
                                    new StatusBarItemViewModel.Message
                                    {
                                        String = StringTags.Message_InOpening,
                                        Options = new object[] {c}
                                    }
                                };
                    }
                })
                .AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsInAuthentication)
                .Subscribe(r =>
                {
                    if (r)
                    {
                        var item = new ProcessingStatusBarItemViewModel(
                            app,
                            StatusBarItemViewModel.SearchKey.IsInAuthentication,
                            StringTags.Message_DuringAuthentication);
                        Messages.Add(item);
                    }
                    else
                    {
                        lock (_messageRemoveLock)
                        {
                            var item = Messages
                                .FirstOrDefault(
                                    x => x.Key == StatusBarItemViewModel.SearchKey.IsInAuthentication);

                            Messages.Remove(item);
                            item?.Dispose();
                        }
                    }
                }).AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsInConnecting)
                .Subscribe(r =>
                {
                    if (r)
                    {
                        var item = new ProcessingStatusBarItemViewModel(
                            app,
                            StatusBarItemViewModel.SearchKey.IsInConnecting,
                            StringTags.Message_Connecting);
                        Messages.Add(item);
                    }
                    else
                    {
                        lock (_messageRemoveLock)
                        {
                            var item = Messages
                                .FirstOrDefault(
                                    x => x.Key == StatusBarItemViewModel.SearchKey.IsInConnecting);

                            Messages.Remove(item);
                            item?.Dispose();
                        }
                    }
                }).AddTo(CompositeDisposable);

            SetupVersionUpdater(app);
        }

        private StatusBarItemViewModel _autoUpdaterItem;

        private void SetupVersionUpdater(App app)
        {
            CompositeDisposable.Add(() => _autoUpdaterItem?.Dispose());

            app.ObserveProperty(x => x.AutoUpdateState)
                .Subscribe(s =>
                {
                    if (_autoUpdaterItem != null)
                    {
                        Messages.Remove(_autoUpdaterItem);
                        _autoUpdaterItem.Dispose();
                    }

                    if (s == App.AutoUpdateStates.CloseAfterNSec)
                    {
                        _autoUpdaterItem =
                            new WaitingStatusBarItemViewModel(
                                app,
                                StringTags.AutoUpdateStates_CloseAfterNSec,
                                new object[] {Constants.AutoUpdateCloseDelaySec});
                    }

                    if (_autoUpdaterItem != null)
                        Messages.Add(_autoUpdaterItem);
                }).AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.AutoUpdateRemainingSeconds)
                .Subscribe(p =>
                {
                    if (app.AutoUpdateState == App.AutoUpdateStates.CloseAfterNSec)
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
                                        String = StringTags.AutoUpdateStates_CloseAfterNSec,
                                        Options = new object[] {p}
                                    }
                                };
                    }
                }).AddTo(CompositeDisposable);
        }
    }
}