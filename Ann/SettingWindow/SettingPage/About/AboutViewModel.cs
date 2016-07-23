using System;
using Ann.Foundation;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.About
{
    public class AboutViewModel : ViewModelBase
    {
        public string Name { get; } = "About";
        public bool IsAbout { get; } = true;

        public string Version { get; }

        public ReactiveCommand<string> OpenUrlCommand { get; }
        public ReactiveCommand<OpenSource> OpenSourceOpenCommand { get; }

        public AboutViewModel()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            OpenUrlCommand = new ReactiveCommand<string>().AddTo(CompositeDisposable);
            OpenUrlCommand.Subscribe(async o => await ProcessHelper.Run(o, string.Empty, false))
                .AddTo(CompositeDisposable);

            OpenSourceOpenCommand = new ReactiveCommand<OpenSource>().AddTo(CompositeDisposable);
            OpenSourceOpenCommand.Subscribe(async o => await ProcessHelper.Run(o.Url, string.Empty, false))
                .AddTo(CompositeDisposable);
        }
    }
}