﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Candidate
{
    public class CalculationResult : ICandidate
    {
        public CalculationResult(string result, LanguagesService languagesService)
        {
            _result = result;
            _languagesService = languagesService;

            _RunCommand = new DelegateCommand(() => Clipboard.SetText(result));
        }

        private readonly string _result;
        private readonly LanguagesService _languagesService;

        string ICandidate.Comment => _languagesService.GetString(StringTags.Calculation);
        Brush ICandidate.Icon => Application.Current?.Resources["IconCalculator"] as Brush;
        string ICandidate.Name => _result;
        MenuCommand[] ICandidate.SubCommands => null;
        bool ICandidate.CanSetPriority => false;

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;
    }
}