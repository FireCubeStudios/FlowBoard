using FlowBoard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class DefaultFileCard : UserControl
    {
        public StorageFile File
        {
            get { return (StorageFile)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }
        public static readonly DependencyProperty FileProperty =
                   DependencyProperty.Register("File", typeof(StorageFile), typeof(DefaultFileCard), null);

        public DefaultFileCard()
        {
            this.InitializeComponent();
            Ring.Visibility = Visibility.Visible;
            Content.Opacity = 0;
        }

        private async void Card_Loaded(object sender, RoutedEventArgs e)
        {
            FileName.Text = File.DisplayName;
            FileDate.Text = "Created: " + File.DateCreated.ToString("MM/dd/yyyy");
            Content.Opacity = 1;
            Ring.Visibility = Visibility.Collapsed;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Content.Opacity = 0;
            await File.DeleteAsync();
            FileHelper.RefreshFiles();
            FileHelper.RefreshRecentItems();
            Content.Opacity = 1;
            Ring.Visibility = Visibility.Collapsed;
        }
    }
}
