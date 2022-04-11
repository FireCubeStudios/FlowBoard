using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using FlowBoard.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FlowBoard.Controls
{
    public sealed partial class InputTools : UserControl
    {
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(StencilTools), null);

        public InputTools() => this.InitializeComponent();

        private void PenToggle_Checked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Pen);

        private void PenToggle_UnChecked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Pen);

        private void CursorToggle_Checked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Mouse);

        private void CursorToggle_UnChecked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Mouse);

        private void TouchToggle_Checked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Touch);

        private void TouchToggle_UnChecked(object sender, RoutedEventArgs e) => Exceptionhelper.CoalesceException(() => inkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Touch);
    }
}
