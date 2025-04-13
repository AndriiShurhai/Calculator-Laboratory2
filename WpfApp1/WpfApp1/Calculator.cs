using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
        }
    }
}
