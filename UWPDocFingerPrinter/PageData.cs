using Windows.Storage;

namespace UWPDocFingerPrinter
{
    public class PageData
    {
        private static PageData instance;

        //MainPage Variables
        public StorageFile MainPageStorageFile { get; private set; }
        public int MainPageRadioBox { get; private set; }

        private PageData()
        {

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
    }
}
