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
        private bool smallView = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PageData data = PageData.Instance();
            if (data.MainPageStorageFile != null)
            {
                fileToEmbed = data.MainPageStorageFile;
                fileNameTextBlock.Text = "Picked photo: " + fileToEmbed.Name;
            }
            int corner = data.MainPageRadioBox;
            switch (corner)
            {
                case 1:
                    topRightButton.IsChecked = true;
                    break;
                case 2:
                    bottomLeftButton.IsChecked = true;
                    break;
                case 3:
                    bottomRightButton.IsChecked = true;
                    break;
                case 4:
                    bottomLeftButton.IsChecked = true;
                    break;
            }

            AlignElements();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            PageData.Instance().SaveMainPageContent(fileToEmbed, getCheckedRadioButton());
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
            this.Frame.Navigate(typeof(FilesPage));
        }

        private void settingsIcon_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
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
            
            if (fileToEmbed != null)
            {
                int corner = getCheckedRadioButton();
                progressRing.Visibility = Visibility.Visible;
                bool success = await HttpRequest.UploadFile(fileToEmbed, corner);
                progressRing.Visibility = Visibility.Collapsed;
            }
                
        }

        private int getCheckedRadioButton()
        {
            int corner = 0;

            if ((bool)topRightButton.IsChecked)
                corner = 1;
            else if ((bool)bottomLeftButton.IsChecked)
                corner = 2;
            else if ((bool)bottomRightButton.IsChecked)
                corner = 3;
            else if ((bool)topLeftButton.IsChecked)
                corner = 4;

            return corner;
        }

        private void AlignElements()
        {
            if (Window.Current.Bounds.Width < 600 && !smallView)
            {
                MySplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                CompactHamburgerButton.Visibility = Visibility.Visible;
                smallView = true;
                embedContent.Width = Window.Current.Bounds.Width - 50;
                RadioBoxGrid.ColumnDefinitions.Remove(RadioBoxGrid.ColumnDefinitions.First());
                RadioBoxGrid.ColumnDefinitions.Remove(RadioBoxGrid.ColumnDefinitions.First());
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1.0, GridUnitType.Star);
                RadioBoxGrid.RowDefinitions.Add(rd);
                RowDefinition rd2 = new RowDefinition();
                rd2.Height = new GridLength(1.0, GridUnitType.Star);
                RadioBoxGrid.RowDefinitions.Add(rd2);
                Grid.SetRow(bottomLeftText, 2);
                Grid.SetRow(bottomLeftButton, 3);
                Grid.SetRow(bottomRightText, 2);
                Grid.SetRow(bottomRightButton, 3);
                Grid.SetColumn(bottomLeftText, 0);
                Grid.SetColumn(bottomLeftButton, 0);
                Grid.SetColumn(bottomRightText, 1);
                Grid.SetColumn(bottomRightButton, 1);
                chooseLocationTextbox.HorizontalAlignment = HorizontalAlignment.Center;
                titleTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else if (smallView)
            {
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
