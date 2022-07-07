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
using FlowBoard.Classes;
using static FlowBoard.Classes.FileClass;
using Windows.UI.Popups;
using Windows.ApplicationModel;
using Windows.UI.Core.Preview;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlowBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<InkDrawingAttributes> Pens = new ObservableCollection<InkDrawingAttributes>();
        ProjectClass Project;
        public MainPage()
        {
            this.InitializeComponent();
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            CanvasSizeService.Initialize(inkCanvas, selectionCanvas, EraserTransform/*, Scroll*/);
            WindowService.Initialize(AppTitleBar);
            CanvasSelectionService.Initialize(inkCanvas, selectionCanvas);
            UndoRedoService.Initialize(UndoButton, RedoButton, inkCanvas);
            SelectionToggle.IsChecked = true;
            Window.Current.Activated += Current_Activated;
            // Application.Current.Suspending += new SuspendingEventHandler(App_Suspending); DISABLED FOR NOW
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            try
            {
                Toolbar.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = false;
                string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                Toolbar.Visibility = Visibility.Visible;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                SaveRing.Visibility = Visibility.Visible;
                await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, true);
            }
            catch
            {
                await new MessageDialog("Project with this name exists or invalid name \n Change the name in settings").ShowAsync();
                Toolbar.Visibility = Visibility.Visible;
                SaveRing.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
            deferral.Complete();
        }

        //Application is suspended, save the file automatically
        public async void App_Suspending(Object sender, SuspendingEventArgs e)
        {
            try
            {
                Toolbar.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = false;
                string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                Toolbar.Visibility = Visibility.Visible;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                SaveRing.Visibility = Visibility.Visible;
                await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, true);
            }
            catch
            {
                // File error or rename is invalid
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Project = e.Parameter as ProjectClass ?? null;
            AppTitle.Text = Project.Name + " - FlowBoard FireCube's Edition PRE-RELEASE";
            if (Project.File.CanvasColor == Colors.Black)
            {
                AddPen(Colors.White);
                AddHighlighter();
            }
            else
            {
                AddPen(Colors.Black);
                AddHighlighter();
            }
            try
            {
                foreach (var i in Project.File.InkStrokes)
                {
                    InkHelper.CreateStroke(i, inkCanvas);
                }
            }
            catch
            {
                // Blank project
            }
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

        private void AddPen_Click(object sender, RoutedEventArgs e) => AddPen(Colors.White);

        public void AddPen(Color color)
        {
            Pens.Add(new InkDrawingAttributes
            {
                Color = color,
                DrawAsHighlighter = false,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Size(12, 12),
                PenTip = PenTipShape.Circle
            });
        }

        private void AddHighlighter_Click(object sender, RoutedEventArgs e) => AddHighlighter();

        public void AddHighlighter()
        {
            Pens.Add(new InkDrawingAttributes
            {
                Color = Colors.Yellow,
                DrawAsHighlighter = true,
                FitToCurve = true,
                IgnorePressure = false,
                IgnoreTilt = false,
                Size = new Size(24, 24),
                PenTip = PenTipShape.Circle
            });
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    Toolbar.Visibility = Visibility.Collapsed;
                    Settings.IsPaneOpen = false;
                    string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                    Toolbar.Visibility = Visibility.Visible;
                    Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                    SaveRing.Visibility = Visibility.Visible;
                    await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, false);
                }
                catch
                {
                    await new MessageDialog("Project with this name exists or invalid name \n Change the name in settings").ShowAsync();
                    Toolbar.Visibility = Visibility.Visible;
                    SaveRing.Visibility = Visibility.Collapsed;
                }
            }));
        }
    }
}
