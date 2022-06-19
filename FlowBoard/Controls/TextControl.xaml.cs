using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
    public sealed partial class TextControl : UserControl
    {
        private bool _isResizing;

        public TextControl()
        {
            this.InitializeComponent();
        }

        private void Manipulator_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.Position.X > Width - ResizeRectangle.Width && e.Position.Y > Height - ResizeRectangle.Height) _isResizing = true;
            else _isResizing = false;
        }
        public static Matrix4x4 ToMatrix4x4(Matrix3x2 matrix)
        {
            return new Matrix4x4(
               matrix.M11, matrix.M12, 0, 0,
               matrix.M21, matrix.M22, 0, 0,
               0, 0, 1, 0,
               matrix.M31, matrix.M32, 0, 1);
        }
        private void Manipulator_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_isResizing)
            {
                Width += e.Delta.Translation.X;
                Height += e.Delta.Translation.Y;
            }
            else
            {
                //  Canvas.SetLeft(this, Canvas.GetLeft(this) + e.Delta.Translation.X);
                // Canvas.SetTop(this, Canvas.GetTop(this) + e.Delta.Translation.Y);
                var scale = Matrix3x2.CreateScale(e.Delta.Scale);
                Matrix3x2 transformX;
                transformX = Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) *
                           scale *
                                      Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
                                      Matrix3x2.CreateTranslation((float)(e.Delta.Translation.X), (float)(e.Delta.Translation.Y));
                this.TransformMatrix *= ToMatrix4x4(transformX);
            }
        }

        private void ContainerGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ResizeRectangle.Visibility = Visibility.Visible;
            UIHelper.IsContentHovered = true;
        }

        private void ContainerGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ResizeRectangle.Visibility = Visibility.Collapsed;
            UIHelper.IsContentHovered = false;
        }
    }
}
