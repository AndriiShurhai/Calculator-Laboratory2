using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace CalculatorProject
{
    class Calculator
    {
        public double PreviousValue { get; set; }
        public double CurrentValue { get; set; }
        public string PendingOperation { get; set; }
        public string DisplayText { get; set; }
        public bool IsNewInput { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public string PrevDisplayText { get; set; }

        public Calculator()
        {
            Clear();
        }

        public void Clear()
        {
            CurrentValue = 0;
            PreviousValue = 0;
            PendingOperation = "";
            DisplayText = "0";
            IsNewInput = true;
            Error = false;
            ErrorMessage = "";
            PrevDisplayText = "";
        }

        public void ClearError()
        {
            Error = false;
            ErrorMessage = "";
        }

        public void SetError(string message)
        {
            Error = true;
            ErrorMessage = message;
            DisplayText = "Error";
        }

        public double PerformOperation(string operation, double firstOperand, double secondOperand)
        {
            try
            {
                switch (operation)
                {
                    case "+":
                        return firstOperand + secondOperand;
                    case "-":
                        return firstOperand - secondOperand;
                    case "*":
                        return firstOperand * secondOperand;
                    case "/":
                        if (secondOperand == 0)
                        {
                            SetError("Cannot divide by zero");
                            return 0;
                        }
                        return firstOperand / secondOperand;
                    case "pow":
                        return Math.Pow(firstOperand, secondOperand);
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                SetError($"Error: {e.Message}");
                return 0;
            }
        }

        public double PerformScientificOperation(string operation)
        {
            try
            {
                switch (operation)
                {
                    case "sqrt":
                        if (CurrentValue < 0)
                        {
                            SetError("Cannot calculate square root of negative number");
                            return 0;
                        }
                        return Math.Sqrt(CurrentValue);

                    case "log":
                        if (CurrentValue <= 0)
                        {
                            SetError("Cannot calculate logarithm of zero or negative number");
                            return 0;
                        }
                        return Math.Log10(CurrentValue);
                    case "ln":
                        if (CurrentValue <= 0)
                        {
                            SetError("Cannot calculate natural logarithm of zero or negative number");
                            return 0;
                        }
                        return Math.Log(CurrentValue);

                    case "pow":
                        return Math.Pow(CurrentValue, 2);

                    default:
                        return CurrentValue;
                }
            }
            catch (Exception ex)
            {
                SetError($"Error: {ex.Message}");
                return 0;
            }
        }

    }
}
