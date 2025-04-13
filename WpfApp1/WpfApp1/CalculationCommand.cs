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

        public CalculationCommand(Calculator calculator, double previousValue, double currentValue, string operation, double result)
        {
            _calculator = calculator;
            _previousValue = previousValue;
            _currentValue = currentValue;
            _operation = operation;
            _result = result;
        }

        public void Execute()
        {
            _calculator.CurrentValue = _result;
            _calculator.DisplayText = _result.ToString();
        }

        public void Undo()
        {
            _calculator.CurrentValue = _previousValue;
            _calculator.DisplayText = _previousValue.ToString();
            _calculator.PendingOperation = _operation;
        }
    }
}
