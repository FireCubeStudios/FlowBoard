using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Data;

namespace FlowBoard.Converters
{
    public class TipToIndexConverter : IValueConverter
    {
        // 0 == circle
        // 1 == rectangle
        public object Convert(object value, Type targetType, object parameter, string language) => (PenTipShape)value == PenTipShape.Circle ? 0 : 1;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => (int)value == 0 ? PenTipShape.Circle : PenTipShape.Rectangle;
    }
}
