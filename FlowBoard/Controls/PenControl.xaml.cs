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

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
                   DependencyProperty.Register("IsSelected", typeof(bool), typeof(PenControl), null);

        public PenControl()
        {
            DrawingAttributes = new InkDrawingAttributes();
            DrawingAttributes.Color = Windows.UI.Colors.White;
            DrawingAttributes.DrawAsHighlighter = false;
            DrawingAttributes.FitToCurve = true;
            DrawingAttributes.IgnorePressure = false;
            DrawingAttributes.IgnoreTilt = false;
            DrawingAttributes.Size = new Windows.Foundation.Size(12, 12);
            DrawingAttributes.PenTip = PenTipShape.Circle;
            this.InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => UpdateCanvas();

        public void UpdateCanvas()
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
            //(sender, e) => UpdateLayout();
        }

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateCanvas();

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args) => UpdateCanvas();
    }
}
