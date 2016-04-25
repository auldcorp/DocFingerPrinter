using Windows.UI;
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
    public sealed partial class DetectionResultsPage : Page
    {
        private bool smallView;
        public DetectionResultsPage()
        {
            this.InitializeComponent();
            SizeChanged += DetectionResultsPage_SizeChanged;
            smallView = false;
        }

        private void DetectionResultsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape) == smallView)
            {
                AlignElements();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PageData data = PageData.Instance();
            if (!string.IsNullOrEmpty(data.DetectionResultsPageUsername))
            {
                UserNameTextBlock.Text = data.DetectionResultsPageUsername;
                ImageIdTextBlock.Text = data.DetectionResultsPageImageNumber.ToString();
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

        private void markedFilesIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
        }

        private void settingsIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(DetectionPage));
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
