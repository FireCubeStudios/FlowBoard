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
            CanvasSizeService.Initialize(inkCanvas, selectionCanvas, EraserTransform/*, Scroll*/);
            WindowService.Initialize(AppTitleBar);
            CanvasSelectionService.Initialize(inkCanvas, selectionCanvas);
            SelectionToggle.IsChecked = true;
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
           // inkCanvas.Height = this.ActualHeight / Scroll.MinZoomFactor;
           // inkCanvas.Width = this.ActualWidth / Scroll.MinZoomFactor;
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
          //  inkCanvas.Height = this.ActualHeight / Scroll.MinZoomFactor;
           // inkCanvas.Width = this.ActualWidth / Scroll.MinZoomFactor;
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
    }
}
