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
using Windows.Storage.AccessCache;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlowBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<InkDrawingAttributes> Pens = new ObservableCollection<InkDrawingAttributes>();
        private ProjectClass Project;
        private string OriginalName;
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
            if (Status.Visibility == Visibility.Visible)
                Project.Name = OriginalName; // Use original name if there is an invalid name
            try
            {
                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested -= App_CloseRequested;
                Toolbar.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = false;
                string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                Toolbar.Visibility = Visibility.Visible;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                SaveRing.Visibility = Visibility.Visible;
                if(Project.Name != OriginalName) //Project has been renamed so remove old file from mru
                {
                    FileHelper.RemoveDuplicate(OriginalName);
                }
                await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, true);
            }
            catch (Exception error)
            {
                await new MessageDialog("PRE-RELEASE: Something went wrong, please report: " + error.Message + " (The file will not be saved during PRE-RELEASE)").ShowAsync();
                Toolbar.Visibility = Visibility.Visible;
                SaveRing.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
            }
            deferral.Complete();
        }

        //Application is suspended, save the file automatically
        public async void App_Suspending(Object sender, SuspendingEventArgs e)
        {
            if (Status.Visibility == Visibility.Visible)
                Project.Name = OriginalName; // Use original name if there is an invalid name
            try
            {
                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested -= App_CloseRequested;
                Toolbar.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = false;
                string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                Toolbar.Visibility = Visibility.Visible;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                SaveRing.Visibility = Visibility.Visible;
                await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, true);
            }
            catch (Exception error)
            {
                await new MessageDialog("PRE-RELEASE: Something went wrong, please report: " + error.Message + " (The file will not be saved during PRE-RELEASE)").ShowAsync();
                Toolbar.Visibility = Visibility.Visible;
                SaveRing.Visibility = Visibility.Collapsed;
                Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Project = e.Parameter as ProjectClass ?? null;
            OriginalName = Project.Name;
            if (Project.File.CanvasColor == Colors.Black || (Application.Current.RequestedTheme == ApplicationTheme.Dark && Project.File.CanvasColor == Colors.Transparent))
            { // Black or Dark Mica
                AddPen(Colors.White);
                AddHighlighter(Colors.Yellow);
            }
            else if(Project.File.CanvasColor == Colors.Wheat) // Wheat
            {
                AddPen(Colors.White);
                AddHighlighter(Colors.LightGreen);
            }
            else // White canvas or Light Mica
            {
                AddPen(Colors.Black);
                AddHighlighter(Colors.Yellow);
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

        private void AddHighlighter_Click(object sender, RoutedEventArgs e) => AddHighlighter(Colors.Yellow);

        public void AddHighlighter(Color color)
        {
            Pens.Add(new InkDrawingAttributes
            {
                Color = color,
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
                if (Status.Visibility == Visibility.Visible)
                    Project.Name = OriginalName; // Use original name if there is an invalid name
               try
               {
                    SystemNavigationManagerPreview.GetForCurrentView().CloseRequested -= App_CloseRequested;
                    Toolbar.Visibility = Visibility.Collapsed;
                    Settings.IsPaneOpen = false;
                    string preview = await FileHelper.SavePreview(inkCanvas, Project.Name);
                    Toolbar.Visibility = Visibility.Visible;
                    Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
                    SaveRing.Visibility = Visibility.Visible;
                    if (Project.Name != OriginalName) //Project has been renamed so remove old file from mru
                    {
                        FileHelper.RemoveDuplicate(OriginalName);
                    }
                    await FileHelper.SaveProjectAsync(((SolidColorBrush)ThemeGrid.Background).Color, inkCanvas, Project, preview, false);
               }
               catch (Exception error)
               {
                    await new MessageDialog("PRE-RELEASE: Something went wrong, please report: " + error.Message + " (The file will not be saved during PRE-RELEASE)").ShowAsync();
                    Toolbar.Visibility = Visibility.Visible;
                    SaveRing.Visibility = Visibility.Collapsed;
                    Settings.IsPaneOpen = (bool)SettingsButton.IsChecked;
               }
            }));
        }

        private async void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Run(() =>
               CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
               {
                   try
                   {
                       if (await FileHelper.IsFilePresent(Name.Text))
                       {
                           Status.Visibility = Visibility.Visible;
                           Status.Text = "Project with the same name already exists";
                           return;
                       }
                       else
                       {
                           Status.Visibility = Visibility.Collapsed;
                           return;
                       }
                   }
                   catch
                   {
                       //Invalid file name
                       Status.Visibility = Visibility.Visible;
                       Status.Text = "Invalid project name";
                       return;
                   }
               })
           );
        }
    }
}
