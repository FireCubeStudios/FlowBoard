using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class EraserHelpers
    {
        //Eraser Properties
        public static int EraserWidth = 4;

        //Points on Cubic Bezier Curve siehe https://www.cubic.org/docs/bezier.htm
        public static Point lerp(Point a, Point b, float t)
        {
            Point p = new Point()
            {
                X = a.X + (b.X - a.X) * t,
                Y = a.Y + (b.Y - a.Y) * t,

            };
            return p;
        }
        public static Point bezier(Point a, Point b, Point c, Point d, float t)
        {
            Point ab = new Point();
            Point bc = new Point();
            Point cd = new Point();
            Point abbc = new Point();
            Point bccd = new Point();

            Point bezierPt = new Point();

            ab = lerp(a, b, t);           // point between a and b
            bc = lerp(b, c, t);           // point between b and c
            cd = lerp(c, d, t);           // point between c and d
            abbc = lerp(ab, bc, t);       // point between ab and bc
            bccd = lerp(bc, cd, t);       // point between bc and cd
            bezierPt = lerp(abbc, bccd, t);   // point on the bezier-curve

            return bezierPt;
        }
        public static List<Point> PointsOnSegment(Point startPt, Point controlPt1, Point controlPt2, Point PositionPt)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < 10; i++)
            {
                Point p = new Point();
                float t = (float)(i) / 9.0f;
                p = bezier(startPt, controlPt1, controlPt2, PositionPt, t);
                points.Add(p);
            }
            return points;
        }
        public static List<Point> GetPointsOnStroke(InkStroke inst)
        {
            List<InkStrokeRenderingSegment> renderingSegments = new List<InkStrokeRenderingSegment>();

            List<Point> points = new List<Point>();
            //First Point on InkStroke
            points.Add(new Point() { X = inst.GetInkPoints().First().Position.X, Y = inst.GetInkPoints().First().Position.Y, });

            foreach (InkStrokeRenderingSegment isrs in inst.GetRenderingSegments())
            {
                List<Point> pointsOnSg = new List<Point>();
                pointsOnSg = PointsOnSegment(points.Last(), isrs.BezierControlPoint1, isrs.BezierControlPoint2, isrs.Position);
                points.AddRange(pointsOnSg);
            }

            return points;
        }
        public static bool PointInRectangle(Point ap, Point ec, int eraserwidth)
        {
            if ((ap.X >= ec.X - eraserwidth && ap.X <= ec.X + eraserwidth) && (ap.Y >= ec.Y - eraserwidth && ap.Y <= ec.Y + eraserwidth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ErasePoints(PointerEventArgs args, InkCanvas inkCanvas)
        {
            List<InkStroke> SelectedStrokes = new List<InkStroke>();
            foreach (InkStroke insr in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
            {
                if (insr.Selected == true)
                {
                    SelectedStrokes.Add(insr);
                }
            }

            InkDrawingAttributes ida;
            List<Point> pointsOnStroke;
            List<Point> PointsA;
            List<Point> PointsB;

            for (int i = 0; i < SelectedStrokes.Count; i++)
            {
                ida = SelectedStrokes[i].DrawingAttributes;
                pointsOnStroke = GetPointsOnStroke(SelectedStrokes[i]);

                PointsA = new List<Point>();
                PointsB = new List<Point>();

                bool IsA = true;

                foreach (Point pt in pointsOnStroke)
                {
                    if (PointInRectangle(pt, args.CurrentPoint.RawPosition, EraserWidth) == true)
                    {
                        IsA = false;
                    }
                    else
                    {
                        if (IsA == true)
                        {
                            PointsA.Add(pt);
                        }
                        else
                        {
                            PointsB.Add(pt);
                        }
                    }
                }

                if (PointsA.Count > 0 || PointsB.Count > 0)
                {
                    var strokeBuilder = new InkStrokeBuilder();
                    strokeBuilder.SetDefaultDrawingAttributes(ida);


                    if (PointsA.Count > 0)
                    {
                        InkStroke stkA = strokeBuilder.CreateStroke(PointsA);
                        inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);

                        if (PointsB.Count > 0)
                        {
                            InkStroke stkB = strokeBuilder.CreateStroke(PointsB);
                            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkB);
                        }
                    }
                    else if (PointsB.Count > 0)
                    {
                        InkStroke stkB = strokeBuilder.CreateStroke(PointsB);
                        inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkB);
                    }
                }
            }
            inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
        }
    }
}
