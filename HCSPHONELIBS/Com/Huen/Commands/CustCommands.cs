using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Com.Huen.Commands
{
    public class CustCommands
    {
        public static RoutedUICommand ExitThisApp;
        public static RoutedUICommand Save2Excel;
        public static RoutedUICommand Print;
        public static RoutedUICommand Options;

        static CustCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.X, ModifierKeys.Control, "Ctrl+X"));
            ExitThisApp = new RoutedUICommand("Exit", "ExitThisApp", typeof(CustCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.P, ModifierKeys.Control, "Ctrl+P"));
            Print = new RoutedUICommand("Print", "Print", typeof(CustCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.E, ModifierKeys.Control, "Ctrl+E"));
            Save2Excel = new RoutedUICommand("Save to Excel", "SavetoExcel", typeof(CustCommands), inputs);

            //inputs = new InputGestureCollection();
            //inputs.Add(new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O"));
            Options = new RoutedUICommand("Options", "Options", typeof(CustCommands), null);
        }
    }
}
