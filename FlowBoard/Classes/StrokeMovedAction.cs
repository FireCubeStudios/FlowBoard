using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input.Inking;

namespace FlowBoard.Classes
{
    public class StrokeMovedAction
    {
        public ActionType Type = ActionType.StrokeMoved;
        public List<InkStroke> MovedStrokes { get; set; }
        public Point Transform;
        public Point InverseTransform;

        public StrokeMovedAction(List<InkStroke> _MovedStroke, Point _Transform)
        {
            MovedStrokes = _MovedStroke;
            Transform = _Transform;
            InverseTransform = new Point(-_Transform.X, -_Transform.Y);
        }
    }
}
