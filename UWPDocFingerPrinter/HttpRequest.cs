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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPDocFingerPrinter
{
    public static class HttpRequest
    {
        public static Cookie authCookie;
        private static string serverHost = "docfingerprint.cloudapp.net";
        private static string localHost = "localhost:51916";
        public async static Task<bool> UploadFile(StorageFile file, int corner)
        {
            bool success = false;
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = serverHost;
            builder.Path = "/ImageUpload/MobileFileUpload";
            var req = (HttpWebRequest)WebRequest.Create(builder.Uri);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.AllowReadStreamBuffering = true;
            CookieContainer cookies = new CookieContainer();
            //cookies.Add(req.RequestUri, authCookie);
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
            //await stream.FlushAsync();
            //stream.Dispose();
            try
            {
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

                    byte[] responseBytes = new byte[randomAccessStream.Size];
                    await randomAccessStream.ReadAsync(responseBytes.AsBuffer(), (uint)responseBytes.Length, InputStreamOptions.None);
                    if (!Directory.Exists(ApplicationData.Current.LocalFolder.Path + "/Images"))
                        await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                    StorageFolder imageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                    StorageFile markedImageFile = await imageFolder.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
                    Stream writeToFile = await markedImageFile.OpenStreamForWriteAsync();
                    await writeToFile.WriteAsync(responseBytes, 0, responseBytes.Length);
                    writeToFile.Dispose();
                    success = true;
                }
            } catch (Exception E)
            {
                Popup errorPopup = new Popup();
                
                success = false;
            }
            return success;
        }

        public async static Task<bool> DetectSignature(StorageFile file, int corner)
        {
            bool success = false;
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = serverHost;
            builder.Path = "/Detection/MobileDetectMark";
            var req = (HttpWebRequest)WebRequest.Create(builder.Uri);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            CookieContainer cookies = new CookieContainer();
            Uri cookieUri = new Uri("http://docFingerprint.cloudapp.net");
            cookies.Add(cookieUri, authCookie);
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
            try
            {
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

                    byte[] responseBytes = new byte[randomAccessStream.Size];
                    await randomAccessStream.ReadAsync(responseBytes.AsBuffer(), (uint)responseBytes.Length, InputStreamOptions.None);
                    string imageString = Encoding.UTF8.GetString(responseBytes);
                    string[] byteToConvert = imageString.Split('.');
                    List<byte> fileBytesList = new List<byte>();
                    byteToConvert.ToList()
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList()
                            .ForEach(x => fileBytesList.Add(Convert.ToByte(x)));

                    byte[] imageBytes = fileBytesList.ToArray();
                    if (!Directory.Exists(ApplicationData.Current.LocalFolder.Path + "/Images"))
                        await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                    StorageFile markedImageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
                    Stream writeToFile = await markedImageFile.OpenStreamForWriteAsync();
                    await writeToFile.WriteAsync(imageBytes, 0, imageBytes.Length);
                    writeToFile.Dispose();
                    success = true;
                }
            }
            catch (Exception E)
            {
                Popup errorPopup = new Popup();

                success = false;
            }
            return success;
        }

        public async static Task<bool> LogIn(string username, string password)
        {
            bool success = false;
            
            CookieContainer cookies = new CookieContainer();
            

            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = serverHost;
            builder.Path = "/Auth/MobileLogIn";

            Uri uri = builder.Uri;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookies;

            try
            {
                var stream = await request.GetRequestStreamAsync();

                StringBuilder serializedBytes = new StringBuilder();

                string postParameters = "email=" + username + "&password=" + password;
                byte[] postData = Encoding.UTF8.GetBytes(postParameters);
                await stream.WriteAsync(postData, 0, postData.Length);
                stream.Dispose();
                HttpWebResponse result = (HttpWebResponse)await request.GetResponseAsync();

                if (result.StatusCode == HttpStatusCode.OK && result.Cookies[".ASPXAUTH"] != null)
                {
                    authCookie = result.Cookies[".ASPXAUTH"];
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("authToken.txt", CreationCollisionOption.ReplaceExisting);
                    StorageFile markedImageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("authToken.txt");
                    Stream writeToFile = await markedImageFile.OpenStreamForWriteAsync();
                    byte[] tokenBytes = Encoding.UTF8.GetBytes(authCookie.Value);
                    await writeToFile.WriteAsync(tokenBytes, 0, tokenBytes.Length);
                    writeToFile.Dispose();

                    success = true;
                }
            }
            catch(Exception E)
            {

            }
            
            return success;
        }
    }
}
