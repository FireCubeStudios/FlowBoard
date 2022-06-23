using FlowBoard.Services;
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
    public sealed partial class ClipboardTools : UserControl
    {
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(EraserControl), null);

        public ClipboardTools()
        {
            this.InitializeComponent();
        }
        private void cutButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.CopySelectedToClipboard();
            inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            CanvasSelectionService.ClearSelection();
        }

        private void copyButton_Click(object sender, RoutedEventArgs e) => inkCanvas.InkPresenter.StrokeContainer.CopySelectedToClipboard();

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
                inkCanvas.InkPresenter.StrokeContainer.PasteFromClipboard(new Point(0, 0));
        }
    }
}
