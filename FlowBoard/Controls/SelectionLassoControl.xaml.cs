using FlowBoard.Classes;
using FlowBoard.Helpers;
using FlowBoard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
    public sealed partial class SelectionLassoControl : UserControl
    {
        public Point AggregateTransform;
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(StencilTools), null);

        public SelectionLassoControl() => this.InitializeComponent();

        public void selection_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var scale = FlowMatrixHelper.GetScale(e);
            var transform = FlowMatrixHelper.GetTranslation(e);
            AggregateTransform.X += transform.Translation.X;
            AggregateTransform.Y += transform.Translation.Y;
            this.TransformMatrix *= FlowMatrixHelper.ToMatrix4x4(transform);
            inkCanvas.InkPresenter.StrokeContainer.MoveSelected(new Point(transform.Translation.X, transform.Translation.Y));
        }

        private void selection_PointerEntered(object sender, PointerRoutedEventArgs e) => UIHelper.IsContentHovered = true;

        private void selection_PointerExited(object sender, PointerRoutedEventArgs e) => UIHelper.IsContentHovered = false;
    }
}
