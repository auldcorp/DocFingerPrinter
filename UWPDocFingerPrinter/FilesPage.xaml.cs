using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
        private Image imageToDisplayMenu;
        private MenuFlyout menu;
        private bool menuIsOpen = false;
        private bool smallView = false;

        public FilesPage()
        {
            this.InitializeComponent();
            SizeChanged += FilesPage_SizeChanged;

            AddImageThumbnailsToLayout();
        }

        private void FilesPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape) == smallView)
            {
                AlignElements();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AlignElements();
        }

        private void ImageToAdd_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender.GetType() == typeof(Image))
                ImageToAdd_CreateDropDownMenu(sender as Image, e.GetPosition(sender as Image));
            else
                throw new Exception("Error: Expected sender to be Image but was: " + sender.GetType().ToString());
        }

        private void ImageToAdd_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (sender.GetType() == typeof(Image))
                ImageToAdd_CreateDropDownMenu(sender as Image, e.GetPosition(sender as Image));
            else
                throw new Exception("Error: Expected sender to be Image but was: " + sender.GetType().ToString());
        }

        private void ImageToAdd_CreateDropDownMenu(Image image, Point cursorOffset)
        {
            imageToDisplayMenu = image;
            if (!menuIsOpen)
            {
                menu.ShowAt(image, cursorOffset);
                menuIsOpen = true;
            }
        }

        private void Menu_Closed(object sender, object e)
        {
            menuIsOpen = false;
        }

        private async void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile imageToOpen = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(imageToDisplayMenu.Name);
                bool launchSucceeded = await Launcher.LaunchFileAsync(imageToOpen);
                if (!(launchSucceeded))
                {
                    //Throw popup saying launch failed
                }
            }
            catch (Exception E)
            {
                //Throw error popup
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShareItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTransferManager dtm = DataTransferManager.GetForCurrentView();
                dtm.DataRequested += Dtm_DataRequested;
                DataTransferManager.ShowShareUI();
            }
            catch (Exception E)
            {
                //Throw error popup
            }
        }

        private async void Dtm_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            StorageFile imageToOpen = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(imageToDisplayMenu.Name);
            List<StorageFile> storageItems = new List<StorageFile>();
            storageItems.Add(imageToOpen);

            DataRequest request = e.Request;
            request.Data.Properties.Title = "Shared from DocFingerPrinterBeta";
            request.Data.Properties.Description = "1 Photo";

            DataRequestDeferral deferral = request.GetDeferral();
            try
            {
                request.Data.SetStorageItems(storageItems);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async void ImageToAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Image tappedImage = sender as Image;
                StorageFile imageToOpen = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(tappedImage.Name);
                bool launchSucceeded = await Launcher.LaunchFileAsync(imageToOpen);
                if (!(launchSucceeded))
                {
                    //Throw popup saying launch failed
                }
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
            Frame.Navigate(typeof(MainPage));
        }

        private void DetectImageIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DetectionPage));
        }


        private void settingsIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private async void AddImageThumbnailsToLayout()
        {
            if (Directory.Exists(ApplicationData.Current.LocalFolder.Path + "\\Images"))
            {
                // Create menu for right clicking/holding images
                menu = new MenuFlyout();
                menu.Placement = FlyoutPlacementMode.Right;
                MenuFlyoutItem openItem = new MenuFlyoutItem();
                openItem.Text = "Open";
                openItem.Click += OpenItem_Click;
                MenuFlyoutItem shareItem = new MenuFlyoutItem();
                shareItem.Text = "Share";
                shareItem.Click += ShareItem_Click;
                MenuFlyoutItem deleteItem = new MenuFlyoutItem();
                deleteItem.Text = "Delete";
                deleteItem.Click += DeleteItem_Click;
                menu.Items.Add(openItem);
                menu.Items.Add(shareItem);
                menu.Items.Add(deleteItem);
                menu.Closed += Menu_Closed;

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
                        var fileToken = StorageApplicationPermissions.FutureAccessList.Add(image);
                        imageToAdd.Name = fileToken;
                        imageToAdd.Source = bitmap;
                        imageToAdd.Margin = new Thickness(1.5);
                        imageToAdd.Tapped += ImageToAdd_Tapped;
                        imageToAdd.Holding += ImageToAdd_Holding;
                        imageToAdd.RightTapped += ImageToAdd_RightTapped;

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
