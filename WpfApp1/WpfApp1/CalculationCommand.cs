using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorProject
{

    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    class CalculationCommand : ICommand
    {
        private readonly Calculator _calculator;
        private readonly double _previousValue;
        private readonly double _currentValue;
        private readonly string _operation;
        private readonly double _result;
        private readonly string _displayBeforeOperation;
        private readonly string _prevDisplayBeforeOperation;

        public CalculationCommand(Calculator calculator, double previousValue, double currentValue, string operation, double result, string displayBeforeOperation, string prevDisplayBeforeOperation)
        {
            _calculator = calculator;
            _previousValue = previousValue;
            _currentValue = currentValue;
            _operation = operation;
            _result = result;
            _displayBeforeOperation = displayBeforeOperation;
            _prevDisplayBeforeOperation = prevDisplayBeforeOperation;
        }

        public void Execute()
        {
            _calculator.PreviousValue = _currentValue;
            _calculator.CurrentValue = _result;
            _calculator.DisplayText = _result.ToString();
            _calculator.PendingOperation = "";
        }

        public void Undo()
        {
            _calculator.PreviousValue = _previousValue;
            _calculator.CurrentValue = _currentValue;
            _calculator.DisplayText = _displayBeforeOperation;
            _calculator.PendingOperation = _operation;
            _calculator.PrevDisplayText = _prevDisplayBeforeOperation;
        }
    }
}
