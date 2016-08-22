using System.Collections.Generic;
using System.Linq;
using Ann.Core;
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

        public class Message
        {
            public StringTags String { get; set; }
            public object[] Options { get; set; }
        }

        public SearchKey Key { get; }
        public ReactiveProperty<Message[]> Messages { get; }
        public App App { get; }

        public StatusBarItemViewModel(App app, IEnumerable<StringTags> messages)
        {
            App = app;
            Key = SearchKey.NoKey;

            Messages = new ReactiveProperty<Message[]>(
                messages.Select(m => new Message {String = m}).ToArray())
                .AddTo(CompositeDisposable);
        }

        public StatusBarItemViewModel(App app, SearchKey key, StringTags str, object[] options = null)
        {
            App = app;
            Key = key;

            Messages = new ReactiveProperty<Message[]>(new [] {new Message
            {
                String = str,
                Options = options
            }}
            ).AddTo(CompositeDisposable);
        }

        public StatusBarItemViewModel(App app, StringTags str, object[] options = null)
            : this(app, SearchKey.NoKey, str, options)
        {
        }
    }

    public class ProcessingStatusBarItemViewModel : StatusBarItemViewModel
    {
        public ProcessingStatusBarItemViewModel(App app, IEnumerable<StringTags> messages)
            : base(app, messages)
        {
        }

        public ProcessingStatusBarItemViewModel(App app, SearchKey key, StringTags message, object[] options = null)
            : base(app, key, message, options)
        {
        }

        public ProcessingStatusBarItemViewModel(App app, StringTags message, object[] options = null)
            : base(app, message, options)
        {
        }
    }

    public class WaitingStatusBarItemViewModel : StatusBarItemViewModel
    {
        public WaitingStatusBarItemViewModel(App app, IEnumerable<StringTags> messages)
            : base(app, messages)
        {
        }

        public WaitingStatusBarItemViewModel(App app, SearchKey key, StringTags message, object[] options = null)
            : base(app, key, message, options)
        {
        }

        public WaitingStatusBarItemViewModel(App app, StringTags message, object[] options = null)
            : base(app, message, options)
        {
        }
    }
}
