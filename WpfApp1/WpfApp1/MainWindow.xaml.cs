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

            if (double.TryParse(number, out double parseResult))
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

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsNewInput)
            {
                _calculator.DisplayText = "0.";
                _calculator.IsNewInput = false;
            }
            else if (_calculator.DisplayText.Contains("."))
            {
               return;
            }

            UpdateDisplay();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _calculator.Clear();
            UpdateDisplay();
        }

        private void ToggleAdvancedPanel_Click(object sender, RoutedEventArgs e)
        {
            _advancedPanelVisible = !_advancedPanelVisible;
            AdvancedPanel.Visibility = _advancedPanelVisible ? Visibility.Visible : Visibility.Collapsed;
            buttonToggleAdvance.Content = _advancedPanelVisible ? "<<" : ">>";
        }
        private void UpdateDisplay()
        {
            txtDisplay.Text = _calculator.DisplayText;
        }
    }
}