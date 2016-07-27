using System;
using System.Threading.Tasks;
using Ann.Core;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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

        public AboutViewModel()
        {

            OpenUrlCommand = new ReactiveCommand<string>().AddTo(CompositeDisposable);
            OpenUrlCommand.Subscribe(async o => await ProcessHelper.Run(o, string.Empty, false))
                .AddTo(CompositeDisposable);

            OpenSourceOpenCommand = new ReactiveCommand<OpenSource>().AddTo(CompositeDisposable);
            OpenSourceOpenCommand.Subscribe(async o => await ProcessHelper.Run(o.Url, string.Empty, false))
                .AddTo(CompositeDisposable);

            VersionCheckingState = _VersionChecker.ObserveProperty(x => x.VersionCheckingState)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }

        public async Task CheckVersion()
        {
            await _VersionChecker.Check();
        }
    }
}