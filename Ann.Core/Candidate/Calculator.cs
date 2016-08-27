using System.Collections.Generic;
using System.Linq;
using Ann.Foundation;

namespace Ann.Core.Candidate
{
    public class Calculator
    {
        public ICandidate Calculate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (input.All(c => AcceptedCharas.Contains(c) == false))
                return null;

            var r = Evaluator.Eval(input);

            if (string.IsNullOrWhiteSpace(r))
                return null;

            return new CalculationResult(r);
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