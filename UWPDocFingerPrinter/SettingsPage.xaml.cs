using System;
using System.IO;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private bool smallView = false;

        public SettingsPage()
        {
            this.InitializeComponent();
            SizeChanged += SettingsPage_SizeChanged;
        }

        private void SettingsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape) == smallView)
            {
                AlignElements();
            }
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

        private void AlignElements()
        {
            if (Window.Current.Bounds.Width < 600 && !smallView)
            {
                smallView = true;
                MySplitView.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else if (smallView)
            {
                smallView = false;
                MySplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
        }

        private void StackPanel_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            panel.Background = new SolidColorBrush(Colors.DarkGray);
        }

        private void StackPanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            panel.Background = new SolidColorBrush(Colors.DimGray);
        }
    }
}
