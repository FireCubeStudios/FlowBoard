using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace FlowBoard.Classes
{
    public class FileInkStroke
    {
        public Color Color { get; set; }
        public bool DrawAsHighlighter { get; set; }
        public bool IgnorePressure { get; set; }
        public bool IgnoreTilt { get; set; }
        public bool IsPenTipCircle { get; set; }
        public List<FileInkPoint> InkPoints { get; set; }
        public Size Size { get; set; }
        public Matrix3x2 PointTransform { get; set; }
    }
}
