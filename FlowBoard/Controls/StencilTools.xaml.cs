using System;
using System.Collections.Generic;
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
    public sealed partial class StencilTools : UserControl
    {
        public InkCanvas inkCanvas
        {
            get { return (InkCanvas)GetValue(inkCanvasProperty); }
            set { SetValue(inkCanvasProperty, value); }
        }
        public static readonly DependencyProperty inkCanvasProperty =
                   DependencyProperty.Register("inkCanvas", typeof(InkCanvas), typeof(StencilTools), null);

        private InkPresenterRuler inkPresenterRuler;
        private InkPresenterProtractor inkPresenterProtractor;

        public StencilTools() => this.InitializeComponent();

        private void ProtractorToggle_Checked(object sender, RoutedEventArgs e) => inkPresenterProtractor.IsVisible = !inkPresenterProtractor.IsVisible;

        private void RulerToggle_Checked(object sender, RoutedEventArgs e) => inkPresenterRuler.IsVisible = !inkPresenterRuler.IsVisible;

        private void StencilTools_Loaded(object sender, RoutedEventArgs e)
        {
            inkPresenterRuler = new InkPresenterRuler(inkCanvas.InkPresenter);
            inkPresenterProtractor = new InkPresenterProtractor(inkCanvas.InkPresenter);
        }
    }
}
