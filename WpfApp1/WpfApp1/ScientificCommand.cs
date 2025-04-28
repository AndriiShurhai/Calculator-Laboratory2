using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorProject
{
    class ScientificCommand : ICommand
    {
        private readonly Calculator _calculator;
        private readonly double _previousValue;
        private readonly string _operation;
        private readonly double _result;
        private readonly string _displayBeforeOperation;

        public ScientificCommand(Calculator calculator, double previousValue, string operation, double result)
        {
            _calculator = calculator;
            _previousValue = previousValue;
            _operation = operation;
            _result = result;
            _displayBeforeOperation = calculator.DisplayText;
        }

        public void Execute()
        {
            _calculator.CurrentValue = _result;
            _calculator.DisplayText = _result.ToString();
            _calculator.PendingOperation = "";
        }

        public void Undo()
        {
            _calculator.CurrentValue = _previousValue;
            _calculator.DisplayText = _displayBeforeOperation;
        }
    }
}