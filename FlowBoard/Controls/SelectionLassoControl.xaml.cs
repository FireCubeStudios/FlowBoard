using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SelectionLassoControl : UserControl
    {
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(StencilTools), null);
        public double LassoWidth
        {
            get { return (double)GetValue(LassoWidthProperty); }
            set { SetValue(LassoWidthProperty, value); }
        }
        public static readonly DependencyProperty LassoWidthProperty =
                   DependencyProperty.Register("LassoWidth", typeof(double), typeof(SelectionLassoControl), null);
        public double Lassoheight
        {
            get { return (double)GetValue(LassoheightProperty); }
            set { SetValue(LassoheightProperty, value); }
        }
        public static readonly DependencyProperty LassoheightProperty =
                   DependencyProperty.Register("Lassoheight", typeof(double), typeof(SelectionLassoControl), null);

        public SelectionLassoControl() => this.InitializeComponent();

        public void selection_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var scale = FlowMatrixHelper.GetScale(e);
            var transform = FlowMatrixHelper.GetTranslation(e);
            this.TransformMatrix *= FlowMatrixHelper.ToMatrix4x4(transform);
            inkCanvas.InkPresenter.StrokeContainer.MoveSelected(new Point(transform.Translation.X, transform.Translation.Y));
           // Canvas.SetLeft(this, Canvas.GetLeft(this) + e.Delta.Translation.X);
           // Canvas.SetTop(this, Canvas.GetLeft(this) + e.Delta.Translation.Y);
        }

        private void selection_PointerEntered(object sender, PointerRoutedEventArgs e) => UIHelper.IsContentHovered = true;

        private void selection_PointerExited(object sender, PointerRoutedEventArgs e) => UIHelper.IsContentHovered = false;
    }
}
