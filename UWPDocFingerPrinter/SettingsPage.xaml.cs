using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPDocFingerPrinter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private SolidColorBrush DarkGrayBrush = new SolidColorBrush(Colors.DarkGray);
        private SolidColorBrush GrayBrush = new SolidColorBrush(Colors.Gray);

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PageData.Instance().SettingsPageApplicationTheme == ApplicationTheme.Dark)
                themeToggle.IsOn = true;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void embedSignatureIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void DetectImageIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DetectionPage));
        }

        private void markedFilesIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
        }

        private async void themeToggle_Toggled(object sender, RoutedEventArgs e)
        { 
            if (themeToggle.IsOn)
                PageData.Instance().SettingsPageApplicationTheme = ApplicationTheme.Dark;
            else
                PageData.Instance().SettingsPageApplicationTheme = ApplicationTheme.Light;

            if (App.Current.RequestedTheme != PageData.Instance().SettingsPageApplicationTheme)
            {
                MessageDialog dialog = new MessageDialog(PageData.Instance().SettingsPageApplicationTheme.ToString() + " theme will be applied when application is restarted!");
                var x = await dialog.ShowAsync();
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(ApplicationData.Current.LocalFolder.Path + "/authToken.txt");
            }
            catch (Exception E)
            {

            }
            Frame.Navigate(typeof(LoginPage));
        }
    }
}
