using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FlowBoard.Controls
{
    public sealed partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                OnPropertyChanged("Color");
            }
        }
        public static readonly DependencyProperty ColorProperty =
                   DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), null);

        public ColorPicker()
        {
            this.InitializeComponent();
            picker.CustomPaletteColors.Add(Colors.Black);
            picker.CustomPaletteColors.Add(Colors.White);
        }
    }
}
