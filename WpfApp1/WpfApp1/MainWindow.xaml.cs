using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalculatorProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

            UpdateDisplay();

            KeyDown += MainWindow_KeyDown;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                HandleNumberInput((e.Key - Key.D0).ToString());
                return;
            }

            switch (e.Key)
            {
                case Key.Add:
                    OperationButton_Click(buttonPlus, new RoutedEventArgs(null, null));
                    break;
                case Key.Subtract:
                    MessageBox.Show("Substract");
                    break;
                case Key.Multiply:
                    MessageBox.Show("Multiply");
                    break;
                case Key.Divide:
                    MessageBox.Show("Divide");
                    break;
                case Key.Enter:
                    MessageBox.Show("Equals");
                    break;
                case Key.OemPlus:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        MessageBox.Show("Addition");
                    else
                        MessageBox.Show("Equals");
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
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string? number = button.Content.ToString();
            HandleNumberInput(number);
        }

        private void HandleNumberInput(string? number)
        {
            if (number == null)
            {
                MessageBox.Show("Number in HandleNumberInput method is null");
                return;
            }
            if (_calculator.IsNewInput)
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
                MessageBox.Show("Can't parse this number");
                return;
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
            if (!string.IsNullOrEmpty(_calculator.PendingOperation) || !_calculator.IsNewInput)
            {
                EqualsButton_Click(null, null);
            }

            _calculator.PreviousValue = _calculator.CurrentValue;
            _calculator.PendingOperation = ((Button)sender).Tag.ToString();
            _calculator.IsNewInput = true;
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
            double result = 0;

            switch (operation)
            {
                case "+":
                    result = previousValue + currentValue;
                    break;
                case "-":
                    result = previousValue - currentValue;
                    break;
                case "*":
                    result = previousValue * currentValue;
                    break;
                case "/":
                    result = previousValue / currentValue;
                    break;
            }

            ICommand command = new CalculationCommand(_calculator, previousValue, currentValue, operation, result);
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
            txtDisplay.Text = _calculator.DisplayText;
        }
    }
}