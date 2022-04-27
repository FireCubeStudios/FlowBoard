using Fluent.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class UIHelper
    {
        public static string GetTitleInfo(Single zoom, double width, double height) => $"SAVED  · {Math.Round(zoom * 10, 0)}%  · {Math.Round(width, 0)} x {Math.Round(height, 0)}";

        public static FluentSymbol PenToIcon(InkDrawingAttributes ink)
        {
            if(ink.Kind == InkDrawingAttributesKind.Pencil)
            {
                return FluentSymbol.CalligraphyPen24;
            }
            else
            {
                return ink.DrawAsHighlighter == true ? FluentSymbol.Highlight24 : FluentSymbol.InkingTool24;
            }
        }
    }
}
