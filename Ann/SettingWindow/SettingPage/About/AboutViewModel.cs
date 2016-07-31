using System;
using System.Threading.Tasks;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Ann.SettingWindow.SettingPage.About
{
    public class AboutViewModel : ViewModelBase
    {
        public string Version { get; }
            = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public ReactiveCommand<string> OpenUrlCommand { get; }
        public ReactiveCommand<OpenSource> OpenSourceOpenCommand { get; }

        private readonly VersionChecker _VersionChecker = new VersionChecker();

        public ReadOnlyReactiveProperty<VersionCheckingStates> VersionCheckingState { get; }
        public ReadOnlyReactiveProperty<int> UpdateProgress { get; }

        public ReactiveCommand RestartCommand { get; }

        public AboutViewModel()
        {
            OpenUrlCommand = new ReactiveCommand<string>().AddTo(CompositeDisposable);
            OpenUrlCommand.Subscribe(async o => await ProcessHelper.RunAsync(o, string.Empty, false))
                .AddTo(CompositeDisposable);

            OpenSourceOpenCommand = new ReactiveCommand<OpenSource>().AddTo(CompositeDisposable);
            OpenSourceOpenCommand.Subscribe(async o => await ProcessHelper.RunAsync(o.Url, string.Empty, false))
                .AddTo(CompositeDisposable);

            VersionCheckingState = _VersionChecker.ObserveProperty(x => x.VersionCheckingState)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            UpdateProgress = VersionUpdater.Instance.ObserveProperty(x => x.UpdateProgress)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            RestartCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            RestartCommand.Subscribe(_ =>
            {
                VersionUpdater.Instance.RequestRestart();
                MessageBroker.Default.Publish(new WindowActionMessage(WindowAction.Close));
            }).AddTo(CompositeDisposable);
        }

        public async Task CheckVersionAsync()
        {
            await _VersionChecker.CheckAsync();
        }
    }
}