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
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(NewProjectPage));
        }

        private async void Recents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AccessListEntry Entry = (AccessListEntry)e.AddedItems[0];
                await FileHelper.OpenProjectAsync(await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(Entry.Token));
            }
            catch
            {

            }  
        }

        private async void Files_SelectionChanged(object sender, SelectionChangedEventArgs e) => await FileHelper.OpenProjectAsync((StorageFile)e.AddedItems[0]);
    }
}
