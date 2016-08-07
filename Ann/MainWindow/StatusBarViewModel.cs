using System.Linq;
using System.Reactive.Linq;
using System.Windows;
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

        public StatusBarViewModel()
        {
            Messages = new ReactiveCollection<StatusBarItemViewModel>().AddTo(CompositeDisposable);
            CompositeDisposable.Add(() => Messages.ForEach(x => x.Dispose()));

            Visibility = Messages.CollectionChangedAsObservable()
                .Select(_ => Messages.Any() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }
}