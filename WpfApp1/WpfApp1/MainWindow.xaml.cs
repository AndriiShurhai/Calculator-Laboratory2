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
            MessageBox.Show("key pressed");
        }
        private void UpdateDisplay()
        {
            txtDisplay.Text = _calculator.DisplayText;
        }
    }
}