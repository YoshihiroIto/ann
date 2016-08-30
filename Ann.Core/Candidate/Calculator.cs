using System.Collections.Generic;
using System.Linq;
using Ann.Foundation;

namespace Ann.Core.Candidate
{
    public class Calculator
    {
        public ICandidate Calculate(string input, LanguagesService languagesService)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (CanAccepte(input) == false)
                return null;

            var r = _Evaluator.Eval(input);

            if (string.IsNullOrWhiteSpace(r))
                return null;

            return new CalculationResult(r, languagesService);
        }

        private readonly Evaluator _Evaluator = new Evaluator();

        public static bool CanAccepte(string input)
        {
            return input.All(c => AcceptedCharas.Contains(c));
        }

        private static readonly HashSet<char> AcceptedCharas = new HashSet<char>
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '+', '-', '*', '/',
            '%',
            '(', ')',
            '.',
            ' '
        };
    }
}