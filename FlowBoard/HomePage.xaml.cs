using FlowBoard.Controls;
using FlowBoard.Helpers;
using FlowBoard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlowBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            WindowService.Initialize(AppTitleBar);
            FileHelper.RefreshRecentItems();
            FileHelper.RefreshFiles();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenRing.Visibility = Visibility.Visible;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(NewProjectPage));
        }

        private async void Recents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenRing.Visibility = Visibility.Visible;
            try
            {
                AccessListEntry Entry = (AccessListEntry)e.AddedItems[0];
                await FileHelper.OpenProjectAsync(await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(Entry.Token));
                OpenRing.Visibility = Visibility.Collapsed;
            }
            catch
            {
                OpenRing.Visibility = Visibility.Collapsed;
              //  await new MessageDialog("PRE-RELEASE Log: Couldn't open file, Please report this issue").ShowAsync();
            }  
        }

        private async void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenRing.Visibility = Visibility.Visible;
            try
            {
                await FileHelper.OpenProjectAsync((StorageFile)e.AddedItems[0]);
                OpenRing.Visibility = Visibility.Collapsed;
            }
            catch
            {
                OpenRing.Visibility = Visibility.Collapsed;
              //  await new MessageDialog("PRE-RELEASE Log: Couldn't open file, Please report this issue").ShowAsync();
            }
        }

        private void OOBE_Click(object sender, RoutedEventArgs e)
        {
            OpenRing.Visibility = Visibility.Visible;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(OOBEPage));
        }

        private async void Discord_Click(object sender, RoutedEventArgs e) => await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.gg/3WYcKat"));
    }
}
