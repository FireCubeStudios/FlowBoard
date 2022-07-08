using FlowBoard.Services;
using Microsoft.Toolkit.Uwp.Helpers;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlowBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeLog : Page
    {
        public ChangeLog()
        {
            this.InitializeComponent();
            WindowService.Initialize(AppTitleBar);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Content.Opacity = 0;
            Frame rootFrame = Window.Current.Content as Frame;
            if (SystemInformation.Instance.IsFirstRun)
            {
                rootFrame.Navigate(typeof(OOBEPage));
            }
            else
            {
                rootFrame.Navigate(typeof(HomePage));
            }
        }
    }
}
