using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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

        private void embedSignature_Click(object sender, RoutedEventArgs e)
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

        private void searchImage_Click(object sender, RoutedEventArgs e)
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

        private void markedFiles_Click(object sender, RoutedEventArgs e)
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

        private void settings_Click(object sender, RoutedEventArgs e)
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
    }
}
