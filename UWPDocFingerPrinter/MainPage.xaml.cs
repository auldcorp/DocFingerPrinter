using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPDocFingerPrinter
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFile fileToEmbed;
        private SolidColorBrush DarkGrayBrush = new SolidColorBrush(Colors.DarkGray);
        private SolidColorBrush GrayBrush = new SolidColorBrush(Colors.Gray);

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void embedSignatureIcon_Click(object sender, RoutedEventArgs e)
        {
            embedPanel.Background = DarkGrayBrush;
            settingPanel.Background = GrayBrush;
            filesPanel.Background = GrayBrush;
            searchPanel.Background = GrayBrush;
            SearchPresenter.IsOpen = false;
            SettingsPresenter.IsOpen = false;
            MarkedFilesPresenter.IsOpen = false;
            embedContent.Opacity = 1;
        }

        private void searchImageIcon_Click(object sender, RoutedEventArgs e)
        {
            embedPanel.Background = GrayBrush;
            settingPanel.Background = GrayBrush;
            filesPanel.Background = GrayBrush;
            searchPanel.Background = DarkGrayBrush;
            SearchPresenter.IsOpen = true;
            SettingsPresenter.IsOpen = false;
            MarkedFilesPresenter.IsOpen = false;
            embedContent.Opacity = 0;
        }

        private void markedFilesIcon_Click(object sender, RoutedEventArgs e)
        {
            embedPanel.Background = GrayBrush;
            settingPanel.Background = GrayBrush;
            filesPanel.Background = DarkGrayBrush;
            searchPanel.Background = GrayBrush;
            SearchPresenter.IsOpen = false;
            SettingsPresenter.IsOpen = false;
            MarkedFilesPresenter.IsOpen = true;
            embedContent.Opacity = 0;
        }

        private void settingsIcon_Click(object sender, RoutedEventArgs e)
        {
            embedPanel.Background = GrayBrush;
            settingPanel.Background = DarkGrayBrush;
            filesPanel.Background = GrayBrush;
            searchPanel.Background = GrayBrush;
            SearchPresenter.IsOpen = false;
            SettingsPresenter.IsOpen = true;
            MarkedFilesPresenter.IsOpen = false;
            embedContent.Opacity = 0;
        }

        private async void imageChooserButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");

                fileToEmbed = await openPicker.PickSingleFileAsync();
                if (fileToEmbed != null)
                {
                    // Application now has read/write access to the picked file
                    fileNameTextBlock.Text = "Picked photo: " + fileToEmbed.Name;
                }
                else
                {
                    fileNameTextBlock.Text = "Operation cancelled.";
                }
            }
        }

        internal static bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                //NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }

            return unsnapped;
        }

        private async void embedSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            int corner = 4;
            if ((bool)topRightButton.IsChecked)
                corner = 1;
            else if ((bool)bottomLeftButton.IsChecked)
                corner = 2;
            else if ((bool)bottomRightButton.IsChecked)
                corner = 3;
            if (fileToEmbed != null)
                await HttpRequest.UploadFile(fileToEmbed, corner);
        }
    }
}
