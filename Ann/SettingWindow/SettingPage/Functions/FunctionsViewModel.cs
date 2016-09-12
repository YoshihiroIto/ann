using System.Diagnostics;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;

namespace Ann.SettingWindow.SettingPage.Functions
{
    public class FunctionsViewModel : ViewModelBase
    {
        public FunctionListBoxViewModel AllFunctions { get; set; }

        public FunctionsViewModel(Core.Config.App model)
        {
            Debug.Assert(model != null);

            AllFunctions = new FunctionListBoxViewModel(model.Functions)
                .AddTo(CompositeDisposable);
        }
    }
}