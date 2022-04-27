using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking;
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
    public partial class PenControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public InkDrawingAttributes DrawingAttributes
        {
            get { return (InkDrawingAttributes)GetValue(DrawingAttributesProperty); }
            set { SetValue(DrawingAttributesProperty, value);
                OnPropertyChanged("drawingAttributes");
            }
        }
        public static readonly DependencyProperty DrawingAttributesProperty =
                   DependencyProperty.Register("DrawingAttributes", typeof(InkDrawingAttributes), typeof(PenControl), null);

        public PenControl() => this.InitializeComponent();

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => UpdateCanvas();

        public void UpdateCanvas()
        {
            try
            {
                SliderCanvas.InkPresenter.StrokeContainer.Clear();
                var strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(DrawingAttributes);
                List<Point> Points;
                Points = new List<Point>();
                Points.Add(new Point(20, 40));
                Points.Add(new Point(65, 10));
                Points.Add(new Point(120, 40));
                Points.Add(new Point(185, 70));
                Points.Add(new Point(250, 40));
                InkStroke stkA = strokeBuilder.CreateStroke(Points);
                SliderCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
                DrawingAttributes = DrawingAttributes;
            }
            catch
            {

            }
        }

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateCanvas();

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args) => UpdateCanvas();
    }
}
