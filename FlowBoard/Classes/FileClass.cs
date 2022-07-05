using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace FlowBoard.Classes
{
    public class FileClass
    {
        public List<FileInkStroke> InkStrokes { get; set; }
        public Color CanvasColor { get; set; }
        public int Version { get; set; }
    }
}
