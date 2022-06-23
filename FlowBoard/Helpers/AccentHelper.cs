using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace FlowBoard.Helpers
{
    public class AccentHelper
    {
        public static SolidColorBrush GetColorFromHex(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16),
                Convert.ToByte(hexaColor.Substring(7, 2), 16)
            ));
        }

        public static SolidColorBrush GetAccentColorLight1() => GetColorFromHex(Application.Current.Resources["SystemAccentColorLight1"].ToString());
    }
}
