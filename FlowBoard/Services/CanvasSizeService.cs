using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace FlowBoard.Services
{
    class CanvasSizeService
    {
        private static float Scale = 1;
        private static InkCanvas inkCanvas;
        private static CompositeTransform EraserTransform;
       // private static ScrollViewer Scroll;

        public static void Initialize(InkCanvas _inkCanvas, Canvas _selectionCanvas, CompositeTransform _EraserTransform/*, ScrollViewer _Scroll*/)
        {
            inkCanvas = _inkCanvas;
            EraserTransform = _EraserTransform;
           // Scroll = _Scroll;
            _selectionCanvas.ManipulationDelta += ink_ManipulationDelta;
            _selectionCanvas.PointerWheelChanged += ink_PointerWheelChanged;
        }

        public static Matrix3x2 GetScaleMatrix() => FlowMatrixHelper.GetScale(Scale);

        private static void ink_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // Return if scaling is too big or small
            if ((e.Delta.Scale > 1 && Scale >= 2.5) || (e.Delta.Scale < 1 && Scale <= 0.2) || UIHelper.IsContentHovered == true)
                return;

            Scale *= e.Delta.Scale;

            var scale = FlowMatrixHelper.GetScale(e);
            var transform = FlowMatrixHelper.GetTranslation(e);
            List<Rect> individualBoundingRects = new List<Rect>();
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            foreach (var stroke in targetStrokes)
            {
                // Don't drag strokes in selection tool
                if (stroke.Selected == false)
                {
                    individualBoundingRects.Add(stroke.BoundingRect);

                    var attr = stroke.DrawingAttributes;
                    // Fix for pencil stroke movement. Avoid being 1 stared in the store.
                    if (attr.Kind != InkDrawingAttributesKind.Pencil)
                    {
                        attr.PenTipTransform *= scale;
                        stroke.DrawingAttributes = attr;
                    }
                    stroke.PointTransform *= transform;
                }
            }
            foreach (var stroke in UndoRedoService.DeletedStrokes)
            {
                individualBoundingRects.Add(stroke.BoundingRect);

                var attr = stroke.DrawingAttributes;
                // Fix for pencil stroke movement. Avoid being 1 stared in the store.
                if (attr.Kind != InkDrawingAttributesKind.Pencil)
                {
                    attr.PenTipTransform *= scale;
                    stroke.DrawingAttributes = attr;
                }
                stroke.PointTransform *= transform;
            }

            InkDrawingAttributes d = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            if (d.Kind != InkDrawingAttributesKind.Pencil)
            {
                d.PenTipTransform *= scale;
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(d);
            }
             EraserTransform.ScaleX *= e.Delta.Scale;
             EraserTransform.ScaleY *= e.Delta.Scale;

            // Legacy code reference
                /*foreach (var i in ContentCanvas.Children)
                {
                    var transformXXX = Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) * 
                                       scale *
                                       Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
                                       Matrix3x2.CreateTranslation((float)e.Delta.Translation.X, (float)e.Delta.Translation.Y);
                    i.TransformMatrix *= ToMatrix4x4(transformXXX);
                }*/
        }

        private static void ink_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var delta = e.GetCurrentPoint(sender as Canvas).Properties.MouseWheelDelta;
            float scale = (float)delta > 0 ? (float)1.04 : (float)0.96;
            // Return if scaling is too big or small
            if ((scale > 1 && Scale >= 2.5) || (scale < 1 && Scale <= 0.2) || UIHelper.IsContentHovered == true)
                return;

            Scale *= scale;

            var scaleMatrix = FlowMatrixHelper.GetScale(scale);
            var transform = FlowMatrixHelper.GetTranslation(e, scale, e.GetCurrentPoint(sender as Canvas));
            List<Rect> individualBoundingRects = new List<Rect>();
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            foreach (var stroke in targetStrokes)
            {
                // Don't drag strokes in selection tool
                if (stroke.Selected == false)
                {
                    individualBoundingRects.Add(stroke.BoundingRect);

                    var attr = stroke.DrawingAttributes;
                    // Fix for pencil stroke movement. Avoid being 1 stared in the store.
                    if (attr.Kind != InkDrawingAttributesKind.Pencil)
                    {
                        attr.PenTipTransform *= scaleMatrix;
                        stroke.DrawingAttributes = attr;
                    }
                    stroke.PointTransform *= transform;
                }
            }
            foreach(var stroke in UndoRedoService.DeletedStrokes)
            {
                individualBoundingRects.Add(stroke.BoundingRect);

                var attr = stroke.DrawingAttributes;
                // Fix for pencil stroke movement. Avoid being 1 stared in the store.
                if (attr.Kind != InkDrawingAttributesKind.Pencil)
                {
                    attr.PenTipTransform *= scaleMatrix;
                    stroke.DrawingAttributes = attr;
                }
                stroke.PointTransform *= transform;
            }

            InkDrawingAttributes d = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            if (d.Kind != InkDrawingAttributesKind.Pencil)
            {
                d.PenTipTransform *= scaleMatrix;
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(d);
            }
            EraserTransform.ScaleX *= scale;
            EraserTransform.ScaleY *= scale;
        }
    }
}
