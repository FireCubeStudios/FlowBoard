using FlowBoard.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class EraserHelper
    {
        public static int EraserWidth = 16;
        private static Matrix3x2 InverseTransform;

        //  Points on Cubic Bezier Curve siehe https://www.cubic.org/docs/bezier.htm
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

        /// <summary>
        /// Gets all the points in the InkStroke segment
        /// </summary>
        /// <param name="startPt">The first ink point in the InkStroke.</param>
        /// <param name="controlPt1">The first BezierControlPoint of the InkStrokeRenderingSegment.</param>
        /// <param name="controlPt2">The second BezierControlPoint of the InkStrokeRenderingSegment.</param>
        /// <param name="PositionPt">Position of the InkStrokeRenderingSegment.</param>
        /// <returns>Returns a list of Point objects representing every Point in the InkStroke segment./returns>
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

        /// <summary>
        /// Gets all the points in the InkStroke
        /// </summary>
        /// <param name="inst">An InkStroke object.</param>
        /// <returns>Returns a list of Point objects representing every Point in the InkStroke./returns>
        public static List<Point> GetPointsOnStroke(InkStroke inst)
        {
            List<InkStrokeRenderingSegment> renderingSegments = new List<InkStrokeRenderingSegment>();
            List<Point> points = new List<Point>();
            points.Add(new Point() { X = inst.GetInkPoints().First().Position.X, Y = inst.GetInkPoints().First().Position.Y, });

            foreach (InkStrokeRenderingSegment isrs in inst.GetRenderingSegments())
            {
                List<Point> pointsOnSg = new List<Point>();
                pointsOnSg = PointsOnSegment(points.Last(), isrs.BezierControlPoint1, isrs.BezierControlPoint2, isrs.Position);
                points.AddRange(pointsOnSg);
            }
            return points;
        }

        /// <summary>
        /// Checks whether an ink stroke point is within the Eraser bounds
        /// </summary>
        /// <param name="ap">An ink stroke point.</param>
        /// <param name="ec">Point with position of the cursor.</param>
        /// <param name="eraserWidth">Width of the Eraser</param>
        /// <returns>Returns bool if the ink stroke point is within the Eraser bounds./returns>
        public static bool PointInRectangle(Point ap, Point ec, int eraserwidth)
        {
            if ((ap.X >= (ec.X + InverseTransform.Translation.X) - eraserwidth &&
                 ap.X <= (ec.X + InverseTransform.Translation.X) + eraserwidth) &&
                 (ap.Y >= (ec.Y + InverseTransform.Translation.Y) - eraserwidth &&
                 ap.Y <= (ec.Y + InverseTransform.Translation.Y) + eraserwidth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Erases points in the selected stroke
        /// </summary>
        /// <param name="args">A PointerEventArgs object.</param>
        /// <param name="inkCanvas">An InkCanvas.</param>
        /// <returns>Returns void./returns>
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
            // Return void if there are no strokes intersecting the eraser
            if (SelectedStrokes.Count == 0)
            {
                return;
            }
            InkDrawingAttributes ida;
            List<Point> pointsOnStroke;
            List<Point> PointsA;
            List<Point> PointsB;      
            for (int i = 0; i < SelectedStrokes.Count; i++)
            {
                ida = SelectedStrokes[i].DrawingAttributes;
                Matrix3x2.Invert(SelectedStrokes[i].PointTransform, out InverseTransform); // Get the inverse transform
                pointsOnStroke = GetPointsOnStroke(SelectedStrokes[i]);
                //  Stroke A
                PointsA = new List<Point>();
                //  Stroke B
                PointsB = new List<Point>();
                //  If the point is in stroke A
                bool IsA = true;
                foreach (Point ii in pointsOnStroke)
                {
                    //  Check if points are within eraser bounds
                    if (PointInRectangle(ii, args.CurrentPoint.RawPosition, EraserWidth) == true)
                    {
                        //  If the point is in the eraser hitbox then the next points should be in stroke B
                        IsA = false;
                    }
                    else
                    {
                        if (IsA == true)
                        {
                            PointsA.Add(ii);
                        }
                        else
                        {
                            PointsB.Add(ii);
                        }
                    }
                }
                //  If the stroke had a PointTransform then remove the first 8 points in Line A
                if (SelectedStrokes[i].PointTransform.Translation.X != 0 && SelectedStrokes[i].PointTransform.Translation.Y != 0)
                {
                    try
                    {
                        PointsA.RemoveRange(0, 8);
                    }
                    catch
                    {
                        try
                        {
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                            PointsA.RemoveAt(0);
                        }
                        catch
                        {

                        }
                    }
                }
                // Draw the remaining strokes
                if (PointsA.Count > 0 || PointsB.Count > 0)
                    {
                        var strokeBuilder = new InkStrokeBuilder();
                        strokeBuilder.SetDefaultDrawingAttributes(ida);
                        //  Draw the first stroke A
                        if (PointsA.Count > 0)
                        {
                            InkStroke stkA = strokeBuilder.CreateStroke(PointsA);
                            //   stkA.PointTransform = OriginalTransform;

                            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
                            //  Draw the stroke B if it exists
                            if (PointsB.Count > 0)
                            {
                                InkStroke stkB = strokeBuilder.CreateStroke(PointsB);
                                //  stkB.PointTransform = OriginalTransform;
                                inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkB);
                            }
                        }
                        //  Draw the stroke B if it exists
                        else if (PointsB.Count > 0)
                        {
                            InkStroke stkB = strokeBuilder.CreateStroke(PointsB);
                            // stkB.PointTransform = OriginalTransform;
                            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkB);
                        }
                    }
                    inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }
    }
}

