using Ann.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage
{
    public class PathViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Path { get; }

        public PathViewModel(Path model)
        {
            Path = model.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(CompositeDisposable);
        }
    }
}