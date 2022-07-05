using FlowBoard.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using static FlowBoard.Classes.FileClass;

namespace FlowBoard.Helpers
{
    public class InkHelper
    {
        public static InkStroke CreateStroke(InkStroke stroke, InkCanvas inkCanvas)
        {
            var strokeBuilder = new InkStrokeBuilder();
            strokeBuilder.SetDefaultDrawingAttributes(stroke.DrawingAttributes);
            IReadOnlyList<InkPoint> inkPoints = stroke.GetInkPoints();
            InkStroke NewStroke = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, stroke.PointTransform);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(NewStroke);
            return NewStroke;
        }

        public static InkStroke CreateStroke(FileInkStroke stroke, InkCanvas inkCanvas)
        {
            var strokeBuilder = new InkStrokeBuilder();
            InkDrawingAttributes attr = new InkDrawingAttributes
            {
                PenTip = stroke.IsPenTipCircle ? PenTipShape.Circle : PenTipShape.Rectangle,
                IgnorePressure = stroke.IgnorePressure,
                IgnoreTilt = stroke.IgnoreTilt,
                Color = stroke.Color,
                DrawAsHighlighter = stroke.DrawAsHighlighter,
                Size = stroke.Size
            };
            strokeBuilder.SetDefaultDrawingAttributes(attr);
            List<InkPoint> inkPoints = new List<InkPoint>();
            foreach(var i in stroke.InkPoints)
            {
                inkPoints.Add(new InkPoint(i.Position, i.Pressure, i.TiltX, i.TiltY, i.TimeStamp));
            }
            InkStroke NewStroke = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, stroke.PointTransform);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(NewStroke);
            return NewStroke;
        }
    }
}
