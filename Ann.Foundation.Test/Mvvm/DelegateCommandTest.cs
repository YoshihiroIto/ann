using System;
using System.Windows.Input;
using Ann.Foundation.Mvvm;
using Xunit;

namespace Ann.Foundation.Test.Mvvm
{
    public class DelegateCommandTest
    {
        [Fact]
        public void Basic()
        {
            var i = 0;

            var a = new Action(() => ++i);
            var c = new DelegateCommand(a);

            Assert.Equal(0, i);

            c.Execute(null);
            
            Assert.Equal(1, i);
        }

        [Fact]
        public void CanExecute()
        {
            var i = 0;

            var a = new Action(() => ++i);

            var b = false;

            // ReSharper disable once AccessToModifiedClosure
            var canExecute = new Func<bool>(() => b);

            var c = new DelegateCommand(a, canExecute);

            Assert.Equal(0, i);

            c.Execute(null);
            Assert.Equal(0, i);

            b = true;

            c.Execute(null);
            Assert.Equal(1, i);
        }

        [WpfFact]
        public void CanExecuteChanged()
        {
            // ReSharper disable once NotAccessedVariable
            var i = 0;

            var a = new Action(() => ++i);
            var c = new DelegateCommand(a);
            c.Execute(null);

            c.CanExecuteChanged += OnCanExecuteChanged;
            CommandManager.InvalidateRequerySuggested();
            c.CanExecuteChanged -= OnCanExecuteChanged;
        }

        private void OnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
        }
    }
}