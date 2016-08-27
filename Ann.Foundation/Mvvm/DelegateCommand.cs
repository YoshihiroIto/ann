﻿using System;
using System.Windows.Input;

namespace Ann.Foundation.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _Execute;
        private readonly Func<bool> _CanExecute;

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Execute();
        }

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }
    }
}