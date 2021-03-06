﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ann.Core;
using Ann.Core.Candidate;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Ann.SettingWindow;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using SimpleInjector;

namespace Ann.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; }
        public ReactiveProperty<bool> IsIgnoreInputChanging { get; }

        public ReactiveCommand IndexUpdateCommand { get; }

        public ReactiveCommand InitializeCommand { get; }

        public ReactiveProperty<CandidatePanelViewModel[]> Candidates { get; }
        public ReactiveProperty<CandidatePanelViewModel> SelectedCandidate { get; }
        public ReactiveCommand<object> SelectedCandidateMoveCommand { get; }

        public ReactiveCommand RunCommand { get; }

        public ReactiveCommand ShowCommand { get; }
        public ReactiveCommand HideCommand { get; }
        public ReactiveCommand ExitCommand { get; }

        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<int> MaxCandidatesLinesCount { get; }

        public ReadOnlyReactiveProperty<double> CandidatesListMaxHeight { get; }

        public AsyncReactiveCommand SettingShowCommand { get; }
        public ReactiveProperty<bool> IsShowingSettingShow { get; }

        public ReadOnlyReactiveProperty<IndexOpeningResults> IndexOpeningResult { get; }

        public WindowMessageBroker Messenger { get; }
        public AsyncMessageBroker AsyncMessenger { get; }

        public string Caption { get; } = $"{AssemblyConstants.Product} {AssemblyConstants.Version}";

        public StatusBarViewModel StatusBar { get; }

        private readonly App _app;

        private const double CandidateItemHeight = 64;

        private readonly HashSet<CandidatePanelViewModel[]> _OldCandidates = new HashSet<CandidatePanelViewModel[]>();
        private readonly object _OldCandidatesLock = new object();

        public MainWindowViewModel(Container diContainer, App app, ConfigHolder configHolder)
        {
            _app = app;

            using (new TimeMeasure("MainWindowViewModel.ctor"))
            {
                Messenger = new WindowMessageBroker().AddTo(CompositeDisposable);
                AsyncMessenger = new AsyncMessageBroker().AddTo(CompositeDisposable);

                InitializeCommand = new ReactiveCommand().AddTo(CompositeDisposable);
                InitializeCommand
                    .Subscribe(async _ => await InitializeAsync(configHolder))
                    .AddTo(CompositeDisposable);

                Left = configHolder.MainWindow.ToReactivePropertyAsSynchronized(x => x.Left).AddTo(CompositeDisposable);
                Top = configHolder.MainWindow.ToReactivePropertyAsSynchronized(x => x.Top).AddTo(CompositeDisposable);

                Input = new ReactiveProperty<string>().AddTo(CompositeDisposable);
                IsIgnoreInputChanging = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

                MaxCandidatesLinesCount = configHolder.Config
                    .ToReactivePropertyAsSynchronized(x => x.MaxCandidateLinesCount)
                    .AddTo(CompositeDisposable);

                CandidatesListMaxHeight =
                    Observable
                        .Merge(MaxCandidatesLinesCount.ToUnit())
                        .Select(_ => CandidateItemHeight*MaxCandidatesLinesCount.Value + 4)
                        .ToReadOnlyReactiveProperty()
                        .AddTo(CompositeDisposable);

                IndexUpdateCommand =
                    Observable
                        .Merge(_app.ObserveProperty(x => x.IsIndexUpdating).ToUnit())
                        .Merge(_app.ObserveProperty(x => x.IndexOpeningResult).ToUnit())
                        .Select(_ =>
                                _app.IsIndexUpdating == false &&
                                _app.IndexOpeningResult != IndexOpeningResults.InOpening
                        )
                        .ToReactiveCommand()
                        .AddTo(CompositeDisposable);

                IndexUpdateCommand
                    .Subscribe(async _ =>
                    {
                        Messenger.Publish(new WindowActionMessage(WindowAction.VisibleActive));
                        await _app.UpdateIndexAsync();
                    })
                    .AddTo(CompositeDisposable);

                Observable
                    .Merge(Input.ToUnit())
                    .Merge(configHolder.Config.ObserveProperty(x => x.MaxCandidateLinesCount).ToUnit())
                    .Where(x => IsIgnoreInputChanging.Value == false)
                    .Subscribe(_ => _app.Find(Input.Value))
                    .AddTo(CompositeDisposable);

                Candidates =
                    new ReactiveProperty<CandidatePanelViewModel[]>(new CandidatePanelViewModel[0]).AddTo(
                        CompositeDisposable);

                _app.ObserveProperty(x => x.Candidates, false)
                    .ObserveOn(ReactivePropertyScheduler.Default)
                    .Subscribe(c =>
                    {
                        lock (_OldCandidatesLock)
                            _OldCandidates.Add(Candidates.Value);

                        Candidates.Value =
                            c.Select(u =>
                            {
                                var p = diContainer.GetInstance<CandidatePanelViewModel>();
                                p.Model = u;
                                return p;
                            }).ToArray();
                    })
                    .AddTo(CompositeDisposable);

                SelectedCandidate = Candidates
                    .Select(c => c?.FirstOrDefault())
                    .ToReactiveProperty()
                    .AddTo(CompositeDisposable);

                SelectedCandidateMoveCommand = SelectedCandidate
                    .Select(c => c != null)
                    .ToReactiveCommand()
                    .AddTo(CompositeDisposable);

                Candidates
                    .Subscribe(c =>
                    {
                        SelectedCandidate.Value = c?.FirstOrDefault();
                        if (SelectedCandidate.Value != null)
                            SelectedCandidate.Value.IsSelected = true;
                    })
                    .AddTo(CompositeDisposable);

                SelectedCandidateMoveCommand
                    .Subscribe(p =>
                    {
                        var current = IndexOfCandidates(SelectedCandidate.Value);
                        var next = current + int.Parse((string) p);

                        if (next == -1)
                            next = Candidates.Value.Length - 1;
                        else if (next == Candidates.Value.Length)
                            next = 0;

                        if (SelectedCandidate.Value != null)
                            SelectedCandidate.Value.IsSelected = false;

                        SelectedCandidate.Value = Candidates.Value[next];
                        SelectedCandidate.Value.IsSelected = true;

                        OnSelectedCandidate(SelectedCandidate.Value);
                    }).AddTo(CompositeDisposable);

                RunCommand = SelectedCandidate
                    .Select(i => i?.RunCommand != null && i.RunCommand.CanExecute(null))
                    .ToReactiveCommand().AddTo(CompositeDisposable);
                RunCommand.Subscribe(_ => SelectedCandidate?.Value?.RunCommand?.Execute(null))
                    .AddTo(CompositeDisposable);

                SettingShowCommand = new AsyncReactiveCommand().AddTo(CompositeDisposable);
                SettingShowCommand.Subscribe(async _ =>
                {
                    using (Disposable.Create(() =>
                    {
                        configHolder.SaveConfig();

                        if (_app.IsRestartRequested)
                        {
                            ExitCommand.Execute(null);
                            return;
                        }

                        IsShowingSettingShow.Value = false;
                        Messenger.Publish(new WindowActionMessage(WindowAction.VisibleActive));
                        _app.InvokeShortcutKeyChanged();
                    }))
                    {
                        var key = configHolder.Config.ShortcutKeys.Activate.Key;
                        configHolder.Config.ShortcutKeys.Activate.Key = Key.None;
                        _app.InvokeShortcutKeyChanged();
                        configHolder.Config.ShortcutKeys.Activate.Key = key;

                        IsShowingSettingShow.Value = true;
                        await AsyncMessenger.PublishAsync(diContainer.GetInstance<SettingViewModel>());
                    }
                }).AddTo(CompositeDisposable);

                IsShowingSettingShow = new ReactiveProperty<bool>().AddTo(CompositeDisposable);

                IsShowingSettingShow
                    .Subscribe(i => _app.IsEnableAutoUpdater = i == false)
                    .AddTo(CompositeDisposable);

                ShowCommand = IsShowingSettingShow.Inverse().ToReactiveCommand().AddTo(CompositeDisposable);
                ShowCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.VisibleActive)))
                    .AddTo(CompositeDisposable);

                HideCommand = new ReactiveCommand().AddTo(CompositeDisposable);
                HideCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.Hidden)))
                    .AddTo(CompositeDisposable);

                ExitCommand = IsShowingSettingShow.Inverse().ToReactiveCommand().AddTo(CompositeDisposable);
                ExitCommand
                    .Subscribe(_ => Messenger.Publish(new WindowActionMessage(WindowAction.Close)))
                    .AddTo(CompositeDisposable);

                IndexOpeningResult = _app.ObserveProperty(x => x.IndexOpeningResult)
                    .ToReadOnlyReactiveProperty()
                    .AddTo(CompositeDisposable);

                IndexOpeningResult
                    .Where(r => r == IndexOpeningResults.Ok)
                    .Subscribe(_ => Input.ForceNotify())
                    .AddTo(CompositeDisposable);

                StatusBar = diContainer.GetInstance<StatusBarViewModel>().AddTo(CompositeDisposable);
            }
        }

        private void OnSelectedCandidate(CandidatePanelViewModel selectedCandidate)
        {
            switch (selectedCandidate.SelectedBehavior)
            {
                case SelectedBehavior.NotAnything:
                    break;

                case SelectedBehavior.UpdateInputWithCommandWord:
                    using (Disposable.Create(() => IsIgnoreInputChanging.Value = false))
                    {
                        IsIgnoreInputChanging.Value = true;
                        Input.Value = $"{selectedCandidate.CommandWord} {selectedCandidate.InputWord}";
                        Messenger.Publish(MessengerMessage.InputTextBoxSetCaretLast);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task InitializeAsync(ConfigHolder configHolder)
        {
            Observable
                .Merge(Left)
                .Merge(Top)
                .Throttle(TimeSpan.FromSeconds(2))
                .Subscribe(_ => configHolder.SaveMainWindow())
                .AddTo(CompositeDisposable);

            configHolder.Config.ObserveProperty(x => x.IconCacheSize)
                .Subscribe(x => _app.ExecutableFileDataBaseIconCacheSize = x)
                .AddTo(CompositeDisposable);

            CompositeDisposable.Add(DisposeCandidates);
            CompositeDisposable.Add(DisposeOldCandidates);

            await _app.OpenIndexAsync();

            if ((IndexOpeningResult.Value == IndexOpeningResults.NotFound) ||
                (IndexOpeningResult.Value == IndexOpeningResults.CanNotOpen) ||
                (IndexOpeningResult.Value == IndexOpeningResults.OldIndex))
                await _app.UpdateIndexAsync();
        }

        public void DisposeOldCandidates()
        {
            lock (_OldCandidatesLock)
            {
                foreach (var old in _OldCandidates)
                    foreach (var c in old)
                        c.Dispose();

                _OldCandidates.Clear();
            }
        }

        private void DisposeCandidates()
        {
            var candidates = Candidates?.Value;

            if (candidates == null)
                return;

            foreach (var c in candidates)
                c.Dispose();
        }

        private int IndexOfCandidates(CandidatePanelViewModel candidate)
        {
            var index = 0;
            foreach (var c in Candidates.Value)
            {
                if (ReferenceEquals(c, candidate))
                    return index;

                ++index;
            }

            return -1;
        }
    }
}