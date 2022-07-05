using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;

namespace FlowBoard.Classes
{
    public class StrokePortionErasedAction
    {
        public ActionType Type = ActionType.StrokePortionErased;
        public InkStroke StrokeA { get; set; }
        public InkStroke StrokeB { get; set; }
        public InkStroke OriginalStroke { get; set; }
        public int StrokeAIndex { get; set; }
        public int StrokeBIndex { get; set; }
        public int OriginalStrokeIndex { get; set; }
        public bool HasBoth { get; set; }

        public StrokePortionErasedAction(InkStroke _A, InkStroke _B, InkStroke _Original)
        {
            StrokeA = _A;
            StrokeB = _B;
            HasBoth = true;
            OriginalStroke = _Original;
        }

        public StrokePortionErasedAction(InkStroke _A, InkStroke _Original)
        {
            StrokeA = _A;
            HasBoth = false;
            OriginalStroke = _Original;
        }

        public StrokePortionErasedAction(int _A, InkStroke _Original)
        {
            StrokeAIndex = _A;
            HasBoth = false;
            OriginalStroke = _Original;
        }

        public StrokePortionErasedAction(InkStroke _A, int _Original)
        {
            StrokeA = _A;
            HasBoth = false;
            OriginalStrokeIndex = _Original;
        }

        public StrokePortionErasedAction(InkStroke _A, InkStroke _B, int _Original)
        {
            StrokeA = _A;
            StrokeB = _B;
            HasBoth = true;
            OriginalStrokeIndex = _Original;
        }

        public StrokePortionErasedAction(int _A, int _B, int _Original)
        {
            StrokeAIndex = _A;
            StrokeBIndex = _B;
            HasBoth = true;
            OriginalStrokeIndex = _Original;
        }

        public StrokePortionErasedAction(int _A, int _B, InkStroke _Original)
        {
            StrokeAIndex = _A;
            StrokeBIndex = _B;
            HasBoth = true;
            OriginalStroke = _Original;
        }

        public StrokePortionErasedAction(int _A, int _Original)
        {
            StrokeAIndex = _A;
            HasBoth = false;
            OriginalStrokeIndex = _Original;
        }
    }
}
