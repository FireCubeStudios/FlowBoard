using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace FlowBoard.Classes
{
    public class FileInkPoint
    {
        public float Pressure { get; set; }
        public float TiltX { get; set; }
        public float TiltY { get; set; }
        public Point Position { get; set; }
        public ulong TimeStamp { get; set; }
    }
}
