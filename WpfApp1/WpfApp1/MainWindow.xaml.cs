using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorProject
{
    public partial class MainWindow : Window
    {
        private Calculator _calculator;
        private Stack<ICommand> _undoStack;
        private Stack<ICommand> _redoStack;
        private bool _advancedPanelVisible;

        public MainWindow()
        {
            InitializeComponent();
            _calculator = new Calculator();
            _undoStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();

            _undoStack.Push(new CalculationCommand(_calculator, 0, 0, "", 0, "0", "0"));
            UpdateDisplay();

            KeyDown += MainWindow_KeyDown;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                int digit;
                if (e.Key >= Key.D0 && e.Key <= Key.D9)
                    digit = e.Key - Key.D0;
                else
                    digit = e.Key - Key.NumPad0;

                HandleNumberInput(digit.ToString());
                return;
            }

            switch (e.Key)
            {
                case Key.Add:
                    OperationButton_Click(buttonPlus, new RoutedEventArgs());
                    break;
                case Key.Subtract:
                    OperationButton_Click(buttonMinus, new RoutedEventArgs());
                    break;
                case Key.Multiply:
                    OperationButton_Click(buttonMultiply, new RoutedEventArgs());
                    break;
                case Key.Divide:
                    OperationButton_Click(buttonDivide, new RoutedEventArgs());
                    break;
                case Key.Enter:
                    EqualsButton_Click(buttonEquals, new RoutedEventArgs());
                    break;
                case Key.OemPlus:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        OperationButton_Click(buttonPlus, new RoutedEventArgs());
                    else
                        EqualsButton_Click(buttonEquals, new RoutedEventArgs());
                    break;
                case Key.OemMinus:
                    MinusButton_Click(buttonMinus, new RoutedEventArgs());
                    break;
                case Key.Escape:
                    ClearButton_Click(buttonC, new RoutedEventArgs());
                    break;
                case Key.Back:
                    EraseSymbol_Click(buttonBackspace, new RoutedEventArgs());
                    break;
                case Key.OemPeriod:
                case Key.Decimal:
                    DecimalButton_Click(buttonDecimal, new RoutedEventArgs());
                    break;
                case Key.Z:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        Undo_Click(null, new RoutedEventArgs());
                    break;
                case Key.Y:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        Redo_Click(null, new RoutedEventArgs());
                    break;
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string? content = button.Content.ToString();
            bool isConstant = false;

            if (content == "Pi")
            {
                content = Math.PI.ToString(CultureInfo.InvariantCulture);
                isConstant = true;
            }
            else if (content == "e")
            {
                content = Math.E.ToString(CultureInfo.InvariantCulture);
                isConstant = true;
            }

            HandleNumberInput(content, isConstant);
        }

        private void HandleNumberInput(string? number, bool isConstant = false)
        {
            if (number == null)
            {
                MessageBox.Show("Number in HandleNumberInput method is null");
                return;
            }

            if (_calculator.Error)
            {
                _calculator.ClearError();
                UpdateDisplay();
            }

            if (isConstant || _calculator.IsNewInput)
            {
                _calculator.DisplayText = number;
                _calculator.IsNewInput = false;
            }
            else
            {
                if (_calculator.DisplayText == "0")
                {
                    _calculator.DisplayText = number;
                }
                else
                {
                    _calculator.DisplayText += number;
                }
            }

            if (double.TryParse(_calculator.DisplayText, out double parseResult))
            {
                _calculator.CurrentValue = parseResult;
            }
            else
            {
                _calculator.SetError("Invalid Input");
            }
            UpdateDisplay();
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsNewInput || _calculator.DisplayText == "0")
            {
                _calculator.DisplayText = "-";
                _calculator.IsNewInput = false;
            }
            else
            {
                OperationButton_Click(sender, e);
            }
            UpdateDisplay();
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsNewInput)
            {
                _calculator.DisplayText = "0.";
                _calculator.IsNewInput = false;
            }
            else if (!_calculator.DisplayText.Contains('.'))
            {
                _calculator.DisplayText += '.';
            }

            if (double.TryParse(_calculator.DisplayText, out double parseResult))
            {
                _calculator.CurrentValue = parseResult;
            }

            UpdateDisplay();
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.Error)
            {
                _calculator.ClearError();
                UpdateDisplay();
                return;
            }

            if (!double.TryParse(_calculator.DisplayText, out double parseResult)) return;

            if (!string.IsNullOrEmpty(_calculator.PendingOperation) && !_calculator.IsNewInput)
            {
                EqualsButton_Click(null, null);
            }

            Button? button = sender as Button;

            if (button != null)
            {

                _calculator.PreviousValue = _calculator.CurrentValue;
                _calculator.CurrentValue = 0;
                _calculator.PendingOperation = button.Tag.ToString();
                _calculator.PrevDisplayText = $"{_calculator.PreviousValue} {_calculator.PendingOperation}";
                _calculator.IsNewInput = true;
            }

            UpdateDisplay();
        }

        private void ScientificButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.Error)
            {
                _calculator.ClearError();
                UpdateDisplay();
                return;
            }

            Button button = sender as Button;
            if (button == null) return;

            string operation = button.Tag.ToString();
            double previousValue = _calculator.CurrentValue;

            double result = _calculator.PerformScientificOperation(operation);

            if (_calculator.Error)
            {
                UpdateDisplay();
                return;
            }

            ICommand command = new ScientificCommand(_calculator, previousValue, operation, result);
            command.Execute();

            _undoStack.Push(command);
            _redoStack.Clear();

            _calculator.IsNewInput = true;

            UpdateDisplay();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _calculator.Clear();
            UpdateDisplay();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_calculator.PendingOperation))
                return;

            double previousValue = _calculator.PreviousValue;
            double currentValue = _calculator.CurrentValue;
            string operation = _calculator.PendingOperation;
            string displayBeforeOperation = _calculator.DisplayText;
            string prevDisplayBeforeOperation = txtOperationIndicator.Text;

            double result = _calculator.PerformOperation(operation, previousValue, currentValue);

            if (_calculator.Error)
            {
                UpdateDisplay();
                return;
            }

            ICommand command = new CalculationCommand(_calculator, previousValue, currentValue, operation, result, displayBeforeOperation, prevDisplayBeforeOperation);
            command.Execute();

            _undoStack.Push(command);
            _redoStack.Clear();

            _calculator.PendingOperation = "";
            _calculator.IsNewInput = true;  

            UpdateDisplay();
        }
        private void EraseSymbol_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsNewInput || string.IsNullOrEmpty(_calculator.DisplayText))
            {
                return;
            }

            if (_calculator.DisplayText.Length > 1)
            {
                _calculator.DisplayText = _calculator.DisplayText.Remove(_calculator.DisplayText.Length - 1);
            }
            else
            {
                _calculator.DisplayText = "0";
                _calculator.IsNewInput = true;
            }

            if (double.TryParse(_calculator.DisplayText, out double parseResult))
            {
                _calculator.CurrentValue = parseResult;
            }
            else
            {
                _calculator.CurrentValue = 0;
            }

            UpdateDisplay();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                if (_calculator.Error)
                {
                    _calculator.ClearError();
                }
                ICommand command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                UpdateDisplay();
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                if (_calculator.Error)
                {
                    _calculator.ClearError();
                }

                ICommand command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                UpdateDisplay() ;
            }
        }

        private void ToggleAdvancedPanel_Click(object sender, RoutedEventArgs e)
        {
            _advancedPanelVisible = !_advancedPanelVisible;
            grid.ColumnDefinitions.ElementAt(1).Width = _advancedPanelVisible ? new GridLength(60) : GridLength.Auto;
            AdvancedPanel.Visibility = _advancedPanelVisible ? Visibility.Visible : Visibility.Collapsed;
            buttonToggleAdvance.Content = _advancedPanelVisible ? "<<" : ">>";
        }
        private void UpdateDisplay()
        {
            if (_calculator.Error)
            {
                txtDisplay.Text = _calculator.ErrorMessage;
                return;
            }

            txtDisplay.Text = _calculator.DisplayText;

            if (!string.IsNullOrEmpty(_calculator.PendingOperation))
            {
                txtOperationIndicator.Text = $"{_calculator.PreviousValue} {_calculator.PendingOperation}";
                txtOperationIndicator.Visibility = Visibility.Visible;
                _calculator.PrevDisplayText = txtOperationIndicator.Text;
            }
            else
            {
                txtOperationIndicator.Visibility = Visibility.Collapsed;
                _calculator.PrevDisplayText = "";  
            }

            debugPanel.Text = $"{_calculator.PreviousValue} {_calculator.CurrentValue} {_calculator.PendingOperation}";
        }
    }
}