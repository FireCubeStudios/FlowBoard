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
using Windows.UI.Core;
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
    public sealed partial class EraserControl : UserControl
    {
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(EraserControl), null);

        public Visibility CanvasVisibility
        {
            get { return (Visibility)GetValue(CanvasVisibilityProperty); }
            set { SetValue(CanvasVisibilityProperty, value); }
        }
        public static readonly DependencyProperty CanvasVisibilityProperty =
                   DependencyProperty.Register("CanvasVisibility", typeof(Visibility), typeof(EraserControl), null);

        public Visibility EraserVisibility
        {
            get { return (Visibility)GetValue(EraserVisibilityProperty); }
            set { SetValue(EraserVisibilityProperty, value); }
        }
        public static readonly DependencyProperty EraserVisibilityProperty =
                   DependencyProperty.Register("EraserVisibility", typeof(Visibility), typeof(EraserControl), null);

        public double TransformX
        {
            get { return (double)GetValue(TransformXProperty); }
            set { SetValue(TransformXProperty, value); }
        }
        public static readonly DependencyProperty TransformXProperty =
                   DependencyProperty.Register("TransformX", typeof(double), typeof(EraserControl), null);

        public double TransformY
        {
            get { return (double)GetValue(TransformYProperty); }
            set { SetValue(TransformYProperty, value); }
        }
        public static readonly DependencyProperty TransformYProperty =
                   DependencyProperty.Register("TransformY", typeof(double), typeof(EraserControl), null);

        public EraserControl()
        {
            TransformY = 0;
            TransformX = 0;
            CanvasVisibility = Visibility.Collapsed;
            EraserVisibility = Visibility.Collapsed;
            this.InitializeComponent();
        }

        private void EraseByPoint_Button_Checked(object sender, RoutedEventArgs e)
        {
            StrokeEraser.IsChecked = false;
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.None;
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.LeaveUnprocessed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
            CanvasVisibility = Visibility.Visible;
            if (CanvasSelectionService.IsSelectionEnabled == true)
            {
                CanvasSelectionService.DisableSelectionSilently();
                CanvasSelectionService.ClearSelection();
            }
        }
        private void EraseByPoint_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.AllowProcessing;
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
            CanvasVisibility = Visibility.Collapsed;
            if (CanvasSelectionService.IsSelectionEnabled == true)
            {
                CanvasSelectionService.EnableSelectionSilently();
            }
        }

        private void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, PointerEventArgs args) => EraserVisibility = Visibility.Collapsed;

        private void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, PointerEventArgs args)
        {
            TransformX = (float)args.CurrentPoint.RawPosition.X - 0.5 * EraserHelper.EraserWidth;
            TransformY = (float)args.CurrentPoint.RawPosition.Y - 0.5 * EraserHelper.EraserWidth;
            EraserVisibility = Visibility.Visible;
        }

        //Points on Cubic Bezier Curve siehe https://www.cubic.org/docs/bezier.htm
        static Matrix3x2 InverseTransform;
        static Matrix3x2 InverseScaleTransform;
        private void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, PointerEventArgs args)
        {
              TransformX = (float)args.CurrentPoint.RawPosition.X - 0.5 * EraserHelper.EraserWidth;
              TransformY = (float)args.CurrentPoint.RawPosition.Y - 0.5 * EraserHelper.EraserWidth;

            //Handle stroke transformation
            foreach (InkStroke insr in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
            {
                if (insr.Selected == true)
                {
                    Matrix3x2.Invert(insr.PointTransform, out InverseTransform);
                    Matrix3x2.Invert(CanvasSizeService.GetScaleMatrix(), out InverseScaleTransform);
                }
            }
            Point p1 = new Point()
            {
                X = (args.CurrentPoint.RawPosition.X + InverseTransform.Translation.X + InverseScaleTransform.Translation.X) - 0.5 * EraserHelper.EraserWidth,
                Y = (args.CurrentPoint.RawPosition.Y + InverseTransform.Translation.X + InverseScaleTransform.Translation.Y) - 0.5 * EraserHelper.EraserWidth,
            };

            Point p2 = new Point()
            {
                X = (args.CurrentPoint.RawPosition.X + InverseTransform.Translation.X + InverseScaleTransform.Translation.X) + 0.5 * EraserHelper.EraserWidth,
                Y = (args.CurrentPoint.RawPosition.Y + InverseTransform.Translation.X + InverseScaleTransform.Translation.Y) - 0.5 * EraserHelper.EraserWidth,
            };

            Point p3 = new Point()
            {
                X = (args.CurrentPoint.RawPosition.X + InverseTransform.Translation.X + InverseScaleTransform.Translation.X) + 0.5 * EraserHelper.EraserWidth,
                Y = (args.CurrentPoint.RawPosition.Y + InverseTransform.Translation.X + InverseScaleTransform.Translation.Y) + 0.5 * EraserHelper.EraserWidth,
            };

            Point p4 = new Point()
            {
                X = (args.CurrentPoint.RawPosition.X + InverseTransform.Translation.X + InverseScaleTransform.Translation.X) - 0.5 * EraserHelper.EraserWidth,
                Y = (args.CurrentPoint.RawPosition.Y + InverseTransform.Translation.X + InverseScaleTransform.Translation.Y) + 0.5 * EraserHelper.EraserWidth,
            };

            inkCanvas.InkPresenter.StrokeContainer.SelectWithLine(p1, p2);
            EraserHelper.ErasePoints(args, inkCanvas);
            inkCanvas.InkPresenter.StrokeContainer.SelectWithLine(p2, p3);
            EraserHelper.ErasePoints(args, inkCanvas);
            inkCanvas.InkPresenter.StrokeContainer.SelectWithLine(p3, p4);
            EraserHelper.ErasePoints(args, inkCanvas);
            inkCanvas.InkPresenter.StrokeContainer.SelectWithLine(p4, p1);
            EraserHelper.ErasePoints(args, inkCanvas);
        }

        private void StrokeEraser_Checked(object sender, RoutedEventArgs e)
        {
            PointEraser.IsChecked = false;
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Erasing;
        }

        private void StrokeEraser_UnChecked(object sender, RoutedEventArgs e) => inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
    }
}
