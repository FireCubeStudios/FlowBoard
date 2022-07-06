using FlowBoard.Classes;
using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FlowBoard.Controls
{
    public sealed partial class RecentFileCard : UserControl
    {
        private StorageFile File;
        private StorageFile previewFile;
        public AccessListEntry Entry
        {
            get { return (AccessListEntry)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
                   DependencyProperty.Register("Entry", typeof(AccessListEntry), typeof(RecentFileCard), null);

        public RecentFileCard()
        {
            this.InitializeComponent();
            Ring.Visibility = Visibility.Visible;
            Content.Opacity = 0;
        }

        private async void Card_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                File = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(Entry.Token);
                previewFile = await ApplicationData.Current.LocalFolder.GetFileAsync(Entry.Metadata);
                using (IRandomAccessStream fileStream = await previewFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    PreviewImage.Source = bitmapImage;
                }
                FileName.Text = File.DisplayName;
                FileDate.Text = "Created: " + File.DateCreated.ToString("MM/dd/yyyy");
                FileClass openedFile = await FileHelper.OpenFileAsync(File);
                imgGrid.Background = new SolidColorBrush(openedFile.CanvasColor);
                Content.Opacity = 1;
                Ring.Visibility = Visibility.Collapsed;
            }
            catch
            {
                // Metadata preview could have failed
                Content.Opacity = 1;
                Ring.Visibility = Visibility.Collapsed;
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Content.Opacity = 0;
            StorageApplicationPermissions.MostRecentlyUsedList.Remove(Entry.Token);
            await File.DeleteAsync();
            await previewFile.DeleteAsync();
            FileHelper.RefreshFiles();
            FileHelper.RefreshRecentItems();
            Content.Opacity = 1;
            Ring.Visibility = Visibility.Collapsed;
        }
    }
}
