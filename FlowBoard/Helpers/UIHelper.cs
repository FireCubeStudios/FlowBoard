using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class UIHelper
    {
        public static string GetTitleInfo(Single zoom, double width, double height) => $"SAVED  · {Math.Round(zoom * 10, 0)}%  · {Math.Round(width, 0)} x {Math.Round(height, 0)}";
    }
}
