using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPDocFingerPrinter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        public FilesPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Directory.Exists(ApplicationData.Current.LocalFolder.Path + "\\Images"))
            {
                StorageFolder imageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                var images = await imageFolder.GetItemsAsync();
                StackPanel horizontalStackPanel = new StackPanel();
                horizontalStackPanel.Orientation = Orientation.Horizontal;
                vertStackPanel.Children.Add(horizontalStackPanel);
                int numImagesInRow = (int)((Window.Current.Bounds.Width - 200) / 200);
                int positionInRow = 0;
                foreach (IStorageItem image in images)
                {
                    if (image.GetType() == typeof(StorageFile))
                    {
                        StorageFile file = image as StorageFile;
                        Image imageToAdd = new Image();
                        BitmapImage bitmap = new BitmapImage();

                        var thumb = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
                        await bitmap.SetSourceAsync(thumb);
                        imageToAdd.Name = image.Name;
                        imageToAdd.Source = bitmap;
                        imageToAdd.Margin = new Thickness(1.5);
                        imageToAdd.Tapped += ImageToAdd_Tapped;
                        if (positionInRow > numImagesInRow)
                        {
                            horizontalStackPanel = new StackPanel();
                            horizontalStackPanel.Orientation = Orientation.Horizontal;
                            vertStackPanel.Children.Add(horizontalStackPanel);
                            positionInRow = 0;
                        }

                        horizontalStackPanel.Children.Add(imageToAdd);
                        positionInRow++;
                    }

                }
            }
        }

        private async void ImageToAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Image tappedImage = sender as Image;
                IStorageFile imageToOpen = await ApplicationData.Current.LocalFolder.GetFileAsync("\\Images\\" + tappedImage.Name);
                Uri launchUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\Images\\" + tappedImage.Name);
                bool x = await Launcher.LaunchUriAsync(launchUri);
            }
            catch (Exception E)
            {
                //Throw error popup
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void embedSignatureIcon_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void searchImageIcon_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private void settingsIcon_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }


    }
}
