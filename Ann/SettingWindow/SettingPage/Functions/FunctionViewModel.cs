using System.Diagnostics;
using Ann.Core;
using Ann.Core.Config;
using Ann.Foundation.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Functions
{
    public class FunctionViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Keyword { get; }

        public ReactiveProperty<StringTags?> ValidationMessage { get; }

        public Function Model { get; }

        public FunctionViewModel(Function model)
        {
            Debug.Assert(model != null);

            Model = model;

            Keyword = model.ToReactivePropertyAsSynchronized(x => x.Keyword).AddTo(CompositeDisposable);

            ValidationMessage = new ReactiveProperty<StringTags?>().AddTo(CompositeDisposable); 
        }
    }
}