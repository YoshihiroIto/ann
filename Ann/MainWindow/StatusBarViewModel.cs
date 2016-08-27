﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
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

        private readonly App _app;

        private readonly object _messageRemoveLock = new object();

        public StatusBarViewModel(App app)
        {
            Debug.Assert(app != null);

            _app = app;

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
                .Select(_ => Messages.Any() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed)
                .ToReadOnlyReactiveProperty(System.Windows.Visibility.Collapsed)
                .AddTo(CompositeDisposable);

            app.ObserveProperty(x => x.IsIndexUpdating)
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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

            app.ObserveProperty(c => c.Crawling)
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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

            SetupVersionUpdater(app);
        }

        private StatusBarItemViewModel _autoUpdaterItem;

        private void SetupVersionUpdater(App app)
        {
            CompositeDisposable.Add(() => _autoUpdaterItem?.Dispose());

            _app.ObserveProperty(x => x.AutoUpdateState)
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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

            _app.ObserveProperty(x => x.AutoUpdateRemainingSeconds)
                //.SubscribeOn(ReactivePropertyScheduler.Default)
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
                                        String = StringTags.AutoUpdateStates_CloseAfterNSec,
                                        Options = new object[] {p}
                                    }
                                };
                    }
                }).AddTo(CompositeDisposable);
        }
    }
}