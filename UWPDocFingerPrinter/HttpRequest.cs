using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPDocFingerPrinter
{
    public static class HttpRequest
    {
        private static Cookie authCookie;
        public async static Task<bool> UploadFile(StorageFile file, int corner)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "docfingerprint.cloudapp.net";
            builder.Path = "/ImageUpload/MobileFileUpload";
            builder.Query = "radio=" + corner;
            var req = (HttpWebRequest)WebRequest.Create(builder.Uri);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //req.Credentials = new NetworkCredential(username, password);
            CookieContainer cookies = new CookieContainer();
            //cookies.Add(builder.Uri, authCookie);
            req.CookieContainer = cookies;
            byte[] fileBytes = null;
            uint fileSize = 0;
            using (IRandomAccessStreamWithContentType fileStream = await file.OpenReadAsync())
            {
                fileSize = (uint)fileStream.Size;
                fileBytes = new byte[fileStream.Size];

                using (DataReader reader = new DataReader(fileStream))
                {
                    await reader.LoadAsync(fileSize);

                    reader.ReadBytes(fileBytes);
                }
            }
            var stream = await req.GetRequestStreamAsync();

            StringBuilder serializedBytes = new StringBuilder();

            fileBytes.ToList().ForEach(x => serializedBytes.AppendFormat("{0}.", Convert.ToUInt32(x)));
            string postParameters = String.Format("fileBytes={0}&fileName={1}&radio={2}", serializedBytes.ToString(), file.Name, corner);
            byte[] postData = Encoding.UTF8.GetBytes(postParameters);
            await stream.WriteAsync(postData, 0, postData.Length);
            stream.Dispose();

            WebResponse result = await req.GetResponseAsync();
            
            if (result.ContentType.Substring(0, 5).Equals("image"))
            {
                var randomAccessStream = new InMemoryRandomAccessStream();
                byte[] byteResponse = new byte[500];
                int read = 0;
                using (var responseStream = result.GetResponseStream())
                {
                    do
                    {
                        read = await responseStream.ReadAsync(byteResponse, 0, byteResponse.Length);
                        await randomAccessStream.WriteAsync(byteResponse.AsBuffer());
                    } while (read != 0);

                    await randomAccessStream.FlushAsync();
                    randomAccessStream.Seek(0);
                }

                byte[] imageBytes = new byte[randomAccessStream.Size];
                await randomAccessStream.ReadAsync(imageBytes.AsBuffer(), (uint)imageBytes.Length, InputStreamOptions.None);

                await ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
                StorageFile markedImageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(file.Name);
                Stream writeToFile = await markedImageFile.OpenStreamForWriteAsync();
                await writeToFile.WriteAsync(imageBytes, 0, imageBytes.Length);
                writeToFile.Dispose();
                return true;
            }

            
            return false;
        }

        public async static Task<bool> LogIn(string username, string password)
        {
            bool success = false;
            
            CookieContainer cookies = new CookieContainer();
            

            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "docfingerprint.cloudapp.net";
            builder.Path = "/Auth/MobileLogIn";

            Uri uri = builder.Uri;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookies;
            var stream = await request.GetRequestStreamAsync();

            StringBuilder serializedBytes = new StringBuilder();

            string postParameters = "email=" + username + "&password=" + password;
            byte[] postData = Encoding.UTF8.GetBytes(postParameters);
            await stream.WriteAsync(postData, 0, postData.Length);
            stream.Dispose();
            HttpWebResponse result =(HttpWebResponse) await request.GetResponseAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {

                if (result.Cookies[".ASPXAUTH"] != null)
                    authCookie = result.Cookies[".ASPXAUTH"];

                success = true;
            }
            return success;
        }
    }
}
