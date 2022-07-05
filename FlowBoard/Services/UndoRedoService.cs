using FlowBoard.Classes;
using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Services
{
    public class UndoRedoService
    {
        private static Stack<dynamic> UndoActionsStack = new Stack<dynamic>();
        private static Stack<dynamic> RedoActionsStack = new Stack<dynamic>();
        public static List<InkStroke> DeletedStrokes = new List<InkStroke>();

        private static Button UndoButton;
        private static Button RedoButton;
        private static InkCanvas inkCanvas;

        public static void Initialize(Button _undoButton, Button _redoButton, InkCanvas _inkCanvas)
        {
            UndoButton = _undoButton;
            RedoButton = _redoButton;
            inkCanvas = _inkCanvas;

            UndoButton.Click += UndoButton_Click;
            RedoButton.Click += RedoButton_Click;
          /*  UndoButton.IsEnabled = UndoActionsStack.Count > 0;
            RedoButton.IsEnabled = RedoActionsStack.Count > 0;*/
            inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            inkCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
        }

        public static void AddUndoAction(dynamic Action)
        {
            UndoActionsStack.Push(Action);
            UndoButton.IsEnabled = UndoActionsStack.Count > 0;
        }

        public static void AddRedoAction(dynamic Action)
        {
            RedoActionsStack.Push(Action);
            RedoButton.IsEnabled = RedoActionsStack.Count > 0;
        }

        private static void UndoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("Undo stack: " + UndoActionsStack.Count);
            Debug.WriteLine("Redo stack: " + RedoActionsStack.Count);
            List<InkStroke> TempSelectedStrokes = new List<InkStroke>();
            dynamic Action = UndoActionsStack.Pop();
            switch(Action.Type)
            {
                //Erase the drawn stroke
                case ActionType.StrokeCollected:
                    Action.Stroke.Selected = true;
                    DeletedStrokes.Add(Action.Stroke);
                    AddRedoAction(new StrokeAction(ActionType.StrokeErased, Action.Stroke, DeletedStrokes.Count - 1));
                    inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    break;
                //Re-draw the erased stroke
                case ActionType.StrokeErased:
                   InkStroke NewStroke = InkHelper.CreateStroke(DeletedStrokes[Action.DeletedIndex], inkCanvas);
                    AddRedoAction(new StrokeAction(ActionType.StrokeCollected, NewStroke));
                    break;
                //Reverse the selected strokes movement
                case ActionType.StrokeMoved:
                    StrokeMovedAction MovedAction = Action;
                    TempSelectedStrokes = new List<InkStroke>();
                    foreach (var i in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
                    {
                        if (i.Selected == true)
                        {
                            TempSelectedStrokes.Add(i);
                        }
                    }
                    foreach (InkStroke i in MovedAction.MovedStrokes)
                    {
                        i.Selected = true;
                    }
                    inkCanvas.InkPresenter.StrokeContainer.MoveSelected(new Point(MovedAction.InverseTransform.X, MovedAction.InverseTransform.Y));
                    foreach (InkStroke i in MovedAction.MovedStrokes)
                    {
                        i.Selected = false;
                    }
                    foreach(var i in TempSelectedStrokes)
                    {
                        i.Selected = true;
                    }
                    AddRedoAction(new StrokeMovedAction(MovedAction.MovedStrokes, MovedAction.InverseTransform));
                    break;
                //Undo the portion erased stroke
                case ActionType.StrokePortionErased:
                    StrokePortionErasedAction ErasedAction = Action;
                    InkStroke Original = InkHelper.CreateStroke(DeletedStrokes[ErasedAction.OriginalStrokeIndex], inkCanvas);
                    TempSelectedStrokes = new List<InkStroke>();
                    foreach (var i in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
                    {
                        if (i.Selected == true)
                        {
                            TempSelectedStrokes.Add(i);
                        }
                    }
                    if (ErasedAction.HasBoth == true)
                    {
                        DeletedStrokes.Add(ErasedAction.StrokeA);
                        DeletedStrokes.Add(ErasedAction.StrokeB);
                        ErasedAction.StrokeA.Selected = true;
                        ErasedAction.StrokeB.Selected = true;
                        inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                        AddRedoAction(new StrokePortionErasedAction(DeletedStrokes.Count - 2, DeletedStrokes.Count - 1, Original));
                    }
                    else
                    {
                        DeletedStrokes.Add(ErasedAction.StrokeA);
                        ErasedAction.StrokeA.Selected = true;
                        inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                        AddRedoAction(new StrokePortionErasedAction(DeletedStrokes.Count - 1, Original));
                    }
                    foreach (var i in TempSelectedStrokes)
                    {
                        i.Selected = true;
                    }
                    break;
            }
            UndoButton.IsEnabled = UndoActionsStack.Count > 0;
            RedoButton.IsEnabled = RedoActionsStack.Count > 0;
            Debug.WriteLine("Undo stack: " + UndoActionsStack.Count);
            Debug.WriteLine("Redo stack: " + RedoActionsStack.Count);
        }

        private static void RedoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("Undo stack: " + UndoActionsStack.Count);
            Debug.WriteLine("Redo stack: " + RedoActionsStack.Count);
            dynamic Action = RedoActionsStack.Pop();
            switch (Action.Type)
            {
                //Erase the recovered stroke
                case ActionType.StrokeCollected:
                    Action.Stroke.Selected = true;
                    DeletedStrokes.Add(Action.Stroke);
                    AddUndoAction(new StrokeAction(ActionType.StrokeErased, Action.Stroke, DeletedStrokes.Count - 1));
                    inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    break;
                //Redo the erased stroke
                case ActionType.StrokeErased:
                    InkStroke NewStroke = InkHelper.CreateStroke(DeletedStrokes[Action.DeletedIndex], inkCanvas);
                    AddUndoAction(new StrokeAction(ActionType.StrokeCollected, NewStroke));
                    break;
                //Redo the undo selected strokes movement
                case ActionType.StrokeMoved:
                    StrokeMovedAction MovedAction = Action;
                    List<InkStroke> TempSelectedStrokes = new List<InkStroke>();
                    foreach (var i in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
                    {
                        if (i.Selected == true)
                        {
                            TempSelectedStrokes.Add(i);
                        }
                    }
                    foreach (InkStroke i in MovedAction.MovedStrokes)
                    {
                        i.Selected = true;
                    }
                    inkCanvas.InkPresenter.StrokeContainer.MoveSelected(new Point(MovedAction.InverseTransform.X, MovedAction.InverseTransform.Y));
                    foreach (InkStroke i in MovedAction.MovedStrokes)
                    {
                        i.Selected = false;
                    }
                    foreach (var i in TempSelectedStrokes)
                    {
                        i.Selected = true;
                    }
                    AddUndoAction(new StrokeMovedAction(MovedAction.MovedStrokes, MovedAction.InverseTransform));
                    break;
                //Redo the undo the portion erased stroke
                case ActionType.StrokePortionErased:
                    StrokePortionErasedAction ErasedAction = Action;
                    List<InkStroke> TempSelectedStrokesE = new List<InkStroke>();
                    foreach (var i in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
                    {
                        if (i.Selected == true)
                        {
                            TempSelectedStrokesE.Add(i);
                        }
                    }
                    InkStroke Original = ErasedAction.OriginalStroke;
                    Original.Selected = true;
                    DeletedStrokes.Add(Original);
                    inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    if (ErasedAction.HasBoth == true)
                    {
                        InkStroke StrokeA = InkHelper.CreateStroke(DeletedStrokes[Action.StrokeAIndex], inkCanvas);
                        InkStroke StrokeB = InkHelper.CreateStroke(DeletedStrokes[Action.StrokeBIndex], inkCanvas);
                        AddUndoAction(new StrokePortionErasedAction(StrokeA, StrokeB, DeletedStrokes.Count - 1));
                    }
                    else
                    {
                        InkStroke StrokeA = InkHelper.CreateStroke(DeletedStrokes[Action.StrokeAIndex], inkCanvas);
                        AddUndoAction(new StrokePortionErasedAction(StrokeA, DeletedStrokes.Count - 1));
                    }
                    foreach (var i in TempSelectedStrokesE)
                    {
                        i.Selected = true;
                    }
                    break;
            }
            UndoButton.IsEnabled = UndoActionsStack.Count > 0;
            RedoButton.IsEnabled = RedoActionsStack.Count > 0;
            Debug.WriteLine("Undo stack: " + UndoActionsStack.Count);
            Debug.WriteLine("Redo stack: " + RedoActionsStack.Count);
        }

        private static void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            foreach (var i in args.Strokes)
            {
                DeletedStrokes.Add(i);
                AddUndoAction(new StrokeAction(ActionType.StrokeErased, i, DeletedStrokes.Count - 1));
            }
        }

        private static void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            foreach(var i in args.Strokes)
            {
                AddUndoAction(new StrokeAction(ActionType.StrokeCollected, i));
            }
        }
    }
}
