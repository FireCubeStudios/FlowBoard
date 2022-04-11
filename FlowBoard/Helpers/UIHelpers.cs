using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class UIHelpers
    {
        public static string GetTitleInfo(ScrollViewer scroll, InkCanvas inkCanvas) => $"SAVED  · {scroll.ZoomFactor * 10}%  · {inkCanvas.Width} x {inkCanvas.Height}";
    }
}
