using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
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

namespace FlowBoard.Services
{
    class CanvasSizeService
    {
        private static float Scale = 1;
        private static InkCanvas inkCanvas;
        private static CompositeTransform EraserTransform;
        private static ScrollViewer Scroll;

        public static void Initialize(InkCanvas _inkCanvas, CompositeTransform _EraserTransform, ScrollViewer _Scroll)
        {
            inkCanvas = _inkCanvas;
            EraserTransform = _EraserTransform;
            Scroll = _Scroll;
            inkCanvas.ManipulationDelta += ink_ManipulationDelta;
        }

        public static Matrix3x2 GetScaleMatrix() => FlowMatrixHelper.GetScale(Scale);

        private static void ink_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // Return if scaling is too big or small
            if ((e.Delta.Scale > 1 && Scale >= 2.5) || (e.Delta.Scale < 1 && Scale <= 0.2))
                return;

            Scale *= e.Delta.Scale;

            var scale = FlowMatrixHelper.GetScale(e);
            var transform = FlowMatrixHelper.GetTranslation(e, Scroll.ZoomFactor);

            List<Rect> individualBoundingRects = new List<Rect>();
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            foreach (var stroke in targetStrokes)
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
    }
}
