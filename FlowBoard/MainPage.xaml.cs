using FlowBoard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using FlowBoard.Controls;
using System.Collections.ObjectModel;
using System.Numerics;
using FlowBoard.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlowBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<InkDrawingAttributes> Pens = new ObservableCollection<InkDrawingAttributes>();
        public MainPage()
        {
            this.InitializeComponent();
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = Windows.UI.Input.Inking.InkInputProcessingMode.Inking;
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            CanvasSizeService.Initialize(inkCanvas);
            WindowService.Initialize(AppTitleBar);
            Pens.Add(new InkDrawingAttributes
            {
                Color = Windows.UI.Colors.White,
                DrawAsHighlighter = false,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Windows.Foundation.Size(8, 8),
                PenTip = PenTipShape.Circle
            });
            Pens.Add(new InkDrawingAttributes
            {
                Color = Windows.UI.Colors.Yellow,
                DrawAsHighlighter = true,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Windows.Foundation.Size(24, 24),
                PenTip = PenTipShape.Circle
            });
            Window.Current.Activated += Current_Activated;
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
         //   inkCanvas.Height = (e.NewSize.Height < inkCanvas.Height) ? inkCanvas.Height : e.NewSize.Height;
          //  inkCanvas.Width = (e.NewSize.Width < inkCanvas.Width) ? inkCanvas.Width : e.NewSize.Width;
        }

        private void PensList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                InkDrawingAttributes d = e.AddedItems[0] as InkDrawingAttributes;
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(d);
            }
            catch
            {

            }
        }

        private void PenControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InkDrawingAttributes d = PensList.SelectedItem as InkDrawingAttributes;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(d);
        }

        private void AddPen_Click(object sender, RoutedEventArgs e)
        {
            Pens.Add(new InkDrawingAttributes
            {
                Color = Windows.UI.Colors.White,
                DrawAsHighlighter = false,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Windows.Foundation.Size(12, 12),
                PenTip = PenTipShape.Circle
            });
        }

        private void AddHighlighter_Click(object sender, RoutedEventArgs e)
        {
            Pens.Add(new InkDrawingAttributes
            {
                Color = Windows.UI.Colors.Yellow,
                DrawAsHighlighter = true,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Windows.Foundation.Size(24, 24),
                PenTip = PenTipShape.Circle
            });
        }

        public static Matrix4x4 ToMatrix4x4(Matrix3x2 matrix)
        {
            return new Matrix4x4(
               matrix.M11, matrix.M12, 0, 0,
               matrix.M21, matrix.M22, 0, 0,
               0, 0, 1, 0,
               matrix.M31, matrix.M32, 0, 1);
        }
        private double Scale = 1;
        private void ink_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (UIHelper.IsContentHovered == false)
            {
                // return if scaling is too big or small
                if(e.Delta.Scale > 1 && Scale >= 2.5)
                {
                    return;
                }
                if (e.Delta.Scale < 1 && Scale <= 0.2)
                {
                    return;
                }
                Scale *= e.Delta.Scale;
                var scale = Matrix3x2.CreateScale(e.Delta.Scale);
                // Matrix3x2.CreateRotation((float)(e.Delta.Rotation / 180 * Math.PI)) *
                var transform = Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) * 
                                scale *
                                Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
                                Matrix3x2.CreateTranslation((float)e.Delta.Translation.X, (float)e.Delta.Translation.Y);
                List<Rect> individualBoundingRects = new List<Rect>();

                var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

                foreach (var stroke in targetStrokes)
                {
                    individualBoundingRects.Add(stroke.BoundingRect);

                    var attr = stroke.DrawingAttributes;
                    // Fix for pencil stroke movement blowup. Avoid being 1 stared in the store.
                    if (attr.Kind != InkDrawingAttributesKind.Pencil)
                    {
                        attr.PenTipTransform *= scale;
                        stroke.DrawingAttributes = attr;
                    }
                    stroke.PointTransform *= transform;
                }
                InkDrawingAttributes d = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                if (d.Kind != InkDrawingAttributesKind.Pencil)
                {
                    d.PenTipTransform *= scale;
                    inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(d);
                }
                TranslateTransform_RectangleEraser.ScaleX *= e.Delta.Scale;
                TranslateTransform_RectangleEraser.ScaleY *= e.Delta.Scale;
                foreach (var i in ContentCanvas.Children)
                {
                    var transformXXX = Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) * 
                                       scale *
                                       Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
                                       Matrix3x2.CreateTranslation((float)e.Delta.Translation.X, (float)e.Delta.Translation.Y);
                    i.TransformMatrix *= ToMatrix4x4(transformXXX);
                }
            }
        }
    }
}
