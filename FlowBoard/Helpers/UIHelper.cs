using Fluent.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlowBoard.Helpers
{
    public class UIHelper
    {
        // Whether the InkCanvas can scale, pan. Disabled when InkStroke is selected
        public static bool IsContentHovered = false;

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

        public static CornerRadius TipToRadius(int index) => index == 0 ? new CornerRadius(12, 12, 12, 12) : new CornerRadius(1, 1, 1, 1);

        public static double PenSizeToUISize(double size) => (0.2 * size) + 4;

        public static bool Invert(bool e) => !e;
    }
}
