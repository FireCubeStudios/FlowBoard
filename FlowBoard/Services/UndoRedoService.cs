using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Services
{
    public class UndoRedoService
    {
        public static Stack<dynamic> UndoActionsStack = new Stack<dynamic>();
        public static Stack<dynamic> RedoActionsStack = new Stack<dynamic>();

        private static Button UndoButton;
        private static Button RedoButton;
        private static InkCanvas inkCanvas;

        public UndoRedoService(Button _undoButton, Button _redoButton, InkCanvas _inkCanvas)
        {
            UndoButton = _redoButton;
            RedoButton = _redoButton;
            inkCanvas = _inkCanvas;

            UndoButton.Click += UndoButton_Click;
            RedoButton.Click += RedoButton_Click;
        }

        private void RedoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
     
        }

        private void UndoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
          
        }
    }
}
