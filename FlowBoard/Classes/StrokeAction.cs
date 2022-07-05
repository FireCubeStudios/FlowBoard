using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;

namespace FlowBoard.Classes
{
    public class StrokeAction
    {
        public ActionType Type { get; set; }
        public InkStroke Stroke { get; set; }
        public int DeletedIndex { get; set; }

        public StrokeAction(ActionType _Type, InkStroke _Stroke)
        {
            Type = _Type;
            Stroke = _Stroke;
        }

        public StrokeAction(ActionType _Type, InkStroke _Stroke, int _Index)
        {
            Type = _Type;
            Stroke = _Stroke;
            DeletedIndex = _Index;
        }
    }
}
