using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace FlowBoard.Controls
{
    public sealed class DeltaControl : Control
    {
        private bool _isResizing;

        private Grid ContainerGrid;

        public Visibility OverlayVisible
        {
            get => (Visibility)GetValue(OverlayVisibleProperty);
            set => SetValue(OverlayVisibleProperty, value);
        }

        DependencyProperty OverlayVisibleProperty = 
            DependencyProperty.Register(nameof(OverlayVisible), typeof(Visibility), typeof(DeltaControl), new PropertyMetadata(default(string)));


        public DeltaControl()
        {
            OverlayVisible = Visibility.Collapsed;
            this.DefaultStyleKey = typeof(DeltaControl);
        }

        protected override void OnApplyTemplate()
        {
            ContainerGrid = GetTemplateChild(nameof(ContainerGrid)) as Grid;
            ContainerGrid.PointerEntered += ContainerGrid_PointerEntered;
            ContainerGrid.PointerExited += ContainerGrid_PointerExited;
            ContainerGrid.ManipulationDelta += Manipulator_OnManipulationDelta;
            ContainerGrid.ManipulationStarted += Manipulator_OnManipulationStarted;
        }

        private void Manipulator_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.Position.X > Width - 16 && e.Position.Y > Height - 16) _isResizing = true;
            else _isResizing = false;
        }

        private void Manipulator_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            try
            {
                if (_isResizing)
                {
                    Width += e.Delta.Translation.X;
                    Height += e.Delta.Translation.Y;
                }
                else
                {
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + e.Delta.Translation.X);
                    Canvas.SetTop(this, Canvas.GetTop(this) + e.Delta.Translation.Y);
                }
            }
            catch
            {

            }
        }

        private void ContainerGrid_PointerEntered(object sender, PointerRoutedEventArgs e) => OverlayVisible = Visibility.Visible;

        private void ContainerGrid_PointerExited(object sender, PointerRoutedEventArgs e) => OverlayVisible = Visibility.Collapsed;
    }
}
