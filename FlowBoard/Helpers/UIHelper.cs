using Fluent.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

        public static bool NullBool(bool? e) => (bool)e;

        public static CornerRadius PaneToRadius(bool e) => e ? new CornerRadius(0, 8, 0, 0) : new CornerRadius(0);

        public static Thickness IndexPaneToThickness(int index, bool pane) => index == 0 && pane ? new Thickness(0, 2, 2, 0) : new Thickness(0);

        public static SolidColorBrush IndexToColor(int index) => index switch
        {
            0 => new SolidColorBrush(Colors.Transparent),
            1 => new SolidColorBrush(Colors.Black),
            2 => new SolidColorBrush(Colors.White),
            3 => new SolidColorBrush(Colors.Wheat),
            _ => new SolidColorBrush(Colors.Transparent)
        };

        public static int ColorToIndex(Color color)
        {
            if(color == Colors.Transparent)
                return 0;
            else if (color == Colors.Black)
                return 1;
            else if (color == Colors.White)
                return 2;
            else if (color == Colors.Wheat)
                return 3;
            else
                return 0;
        }

        public static bool TextToBool(string text) => text != "";

        public static Visibility ItemsToVisibility(ItemCollection collection) => collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }
}
