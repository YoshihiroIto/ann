using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.MainWindow
{
    public class StatusBarItemViewModel : ViewModelBase
    {
        public enum SearchKey
        {
            IndexUpdating,
            ActivationShortcutKeyIsAlreadyInUse,
            InOpening,
            //
            NoKey
        }

        public ReactiveProperty<string> Message { get; }
        public SearchKey Key { get; }

        public StatusBarItemViewModel(string message)
        {
            Message = new ReactiveProperty<string>(message).AddTo(CompositeDisposable);
            Key = SearchKey.NoKey;
        }

        public StatusBarItemViewModel(SearchKey key, string message)
        {
            Message = new ReactiveProperty<string>(message).AddTo(CompositeDisposable);
            Key = key;
        }
    }

    public class ProcessingStatusBarItemViewModel : StatusBarItemViewModel
    {
        public ProcessingStatusBarItemViewModel(string message)
            : base(message)
        {
        }

        public ProcessingStatusBarItemViewModel(SearchKey key, string message)
            : base(key, message)
        {
        }
    }
}