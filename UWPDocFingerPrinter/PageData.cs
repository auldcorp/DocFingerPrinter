using Windows.Storage;
using Windows.UI.Xaml;

namespace UWPDocFingerPrinter
{
    public class PageData
    {
        private static PageData instance;

        //MainPage Variables
        public StorageFile MainPageStorageFile { get; private set; }
        public int MainPageRadioBox { get; private set; }
        public ApplicationTheme SettingsPageApplicationTheme { get; set; }
        public StorageFile DetectionPageStorageFile { get; private set; }
        public int DetectionPageRadioBox { get; private set; }
        public int DetectionResultsPageImageNumber { get; private set; }
        public string DetectionResultsPageUsername { get; private set; }
        private PageData()
        {
            MainPageStorageFile = DetectionPageStorageFile = null;
            MainPageRadioBox = DetectionPageRadioBox = 0;
            SettingsPageApplicationTheme = ApplicationTheme.Light;
        }

        public static PageData Instance()
        {
            if (instance == null)
            {
                instance = new PageData();
            }
            return instance;
        }

        public void SaveMainPageContent(StorageFile file, int radioBox)
        {
            MainPageStorageFile = file;
            MainPageRadioBox = radioBox;
        }

        public void SaveDetectionPageContent(StorageFile file, int radioBox)
        {
            MainPageStorageFile = file;
            MainPageRadioBox = radioBox;
        }

        public void SetDetectionResultsPageData(string user, int imageNumber)
        {
            DetectionResultsPageUsername = user;
            DetectionResultsPageImageNumber = imageNumber;
        }
    }
}
