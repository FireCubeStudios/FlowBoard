using FlowBoard.Classes;
using FlowBoard.Controls;
using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace FlowBoard.Services
{
    public class CanvasSelectionService
    {
        private static SelectionLassoControl BoundingLasso;
        private static Polyline lasso;
        private static Rect boundingRect;
        private static InkCanvas inkCanvas;
        private static Canvas selectionCanvas;
        public static bool IsSelectionEnabled = true;

        public static void Initialize(InkCanvas _inkCanvas, Canvas _selectionCanvas)
        {
            inkCanvas = _inkCanvas;
            selectionCanvas = _selectionCanvas;
            inkCanvas.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
        }

        public static void EnableSelection(object sender, RoutedEventArgs e)
        {
            // By default, the InkPresenter processes input modified by 
            // a secondary affordance (pen barrel button, right mouse 
            // button, or similar) as ink.
            // To pass through modified input to the app for custom processing 
            // on the app UI thread instead of the background ink thread, set 
            // InputProcessingConfiguration.RightDragAction to LeaveUnprocessed.
            ExceptionHelper.CoalesceException(() => inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.LeaveUnprocessed);

            // Listen for unprocessed pointer events from modified input.
            // The input is used to provide selection functionality.
            try
            {
                inkCanvas.InkPresenter.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
                inkCanvas.InkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
                inkCanvas.InkPresenter.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
            }
            catch
            {

            }
            IsSelectionEnabled = true;
        }

        public static void DisableSelection(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.AllowProcessing;
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved -=UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
            IsSelectionEnabled = false;
        }

        public static void DisableSelectionSilently()
        {
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.AllowProcessing;
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
        }

        public static void EnableSelectionSilently()
        {
            ExceptionHelper.CoalesceException(() => inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.LeaveUnprocessed);
            try
            {
                inkCanvas.InkPresenter.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
                inkCanvas.InkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
                inkCanvas.InkPresenter.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
            }
            catch
            {

            }
        }

        // Handle unprocessed pointer events from modifed input.
        // The input is used to provide selection functionality.
        // Selection UI is drawn on a canvas under the InkCanvas.
        private static void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Initialize a selection lasso.
            lasso = new Polyline()
            {
                Stroke = AccentHelper.GetAccentColorLight1(),
                StrokeThickness = 4,
                StrokeDashArray = new DoubleCollection() { 5, 2 },
            };

            lasso.Points.Add(args.CurrentPoint.RawPosition);

            selectionCanvas.Children.Add(lasso);
        }



        private static void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Add a point to the lasso Polyline object.
            lasso.Points.Add(args.CurrentPoint.RawPosition);
        }

        private static void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Add the final point to the Polyline object and 
            // select strokes within the lasso area.
            // Draw a bounding box on the selection canvas 
            // around the selected ink strokes.
            lasso.Points.Add(args.CurrentPoint.RawPosition);

            boundingRect =
                inkCanvas.InkPresenter.StrokeContainer.SelectWithPolyLine(
                    lasso.Points);
            DrawBoundingRect();
        }

        // Draw a bounding rectangle, on the selection canvas, encompassing 
        // all ink strokes within the lasso area.
        private static void DrawBoundingRect()
        {
            // Clear all existing content from the selection canvas.
            selectionCanvas.Children.Clear();

            // Draw a bounding rectangle only if there are ink strokes 
            // within the lasso area.
            if (!((boundingRect.Width == 0) || (boundingRect.Height == 0) || boundingRect.IsEmpty))
            {
                BoundingLasso = new SelectionLassoControl()
                {
                    Width = boundingRect.Width,
                    Height = boundingRect.Height,
                    inkCanvas = inkCanvas,
                };
                Canvas.SetLeft(BoundingLasso, boundingRect.X);
                Canvas.SetTop(BoundingLasso, boundingRect.Y);

                selectionCanvas.Children.Add(BoundingLasso);
            }
        }

        // Clean up selection UI.
        public static void ClearSelection()
        {
            try
            {
                List<InkStroke> MovedStrokes = new List<InkStroke>();
                foreach (var i in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
                {
                    if (i.Selected == true)
                    {
                        MovedStrokes.Add(i);
                    }
                }
                UndoRedoService.AddUndoAction(new StrokeMovedAction(MovedStrokes, BoundingLasso.AggregateTransform));
            }
            catch
            {

            }
            var strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            foreach (var stroke in strokes)
            {
                stroke.Selected = false;
            }
            ClearBoundingRect();
        }

        private static void ClearBoundingRect()
        {
            if (selectionCanvas.Children.Any())
            {
                selectionCanvas.Children.Clear();
                boundingRect = Rect.Empty;
            }
        }

        private static void StrokeInput_StrokeStarted(InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args) => ClearSelection();

        private static void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args) => ClearSelection();
    }
}
