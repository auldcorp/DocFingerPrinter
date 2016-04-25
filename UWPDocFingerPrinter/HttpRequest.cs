using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Primitives;

namespace UWPDocFingerPrinter
{
    public static class HttpRequest
    {
        public static Cookie authCookie;
        private static string serverHost = "docfingerprint.cloudapp.net";
        private static string localHost = "localhost";
        public async static Task<bool> UploadFile(StorageFile file, int corner, bool transparentSignatureBackground)
        {
            bool success = false;
                        
            HttpWebRequest req = CreateWebRequest("/ImageUpload/MobileFileUpload", true);
            req.AllowReadStreamBuffering = true;
            try
            {
                var randomAccessStream = await file.OpenReadAsync();
                var readStream = randomAccessStream.AsStreamForRead();
                byte[] fileBytes = new byte[readStream.Length];
                await readStream.ReadAsync(fileBytes, 0, fileBytes.Length);

                StringBuilder serializedBytes = new StringBuilder();
                fileBytes.ToList().ForEach(x => serializedBytes.AppendFormat("{0}.", Convert.ToUInt32(x)));
                string postParameters = String.Format("fileBytes={0}&fileName={1}&radio={2}&transparentBackground={3}", serializedBytes.ToString(), file.Name, corner, transparentSignatureBackground);
                byte[] postData = Encoding.UTF8.GetBytes(postParameters);

                var stream = await req.GetRequestStreamAsync();
                await stream.WriteAsync(postData, 0, postData.Length);
                stream.Dispose();

                 WebResponse result = await req.GetResponseAsync();
                if (result.ContentType.Substring(0, 5).Equals("image"))
                {
                    var responseStream = result.GetResponseStream();
                    byte[] byteResponse = new byte[responseStream.Length];
                    await responseStream.ReadAsync(byteResponse, 0, byteResponse.Length);
                    responseStream.Dispose();

                    if (!Directory.Exists(ApplicationData.Current.LocalFolder.Path + @"\Images"))
                        await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

                    StorageFolder imageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                    StorageFile markedImageFile = await imageFolder.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
                    File.WriteAllBytes(markedImageFile.Path, byteResponse);
                    success = true;
                }
            }
            catch (Exception E)
            {
                Popup errorPopup = new Popup();
                
                
            }
            return success;
        }

        public async static Task<bool> DetectSignature(StorageFile file, int corner)
        {
            bool success = false;

            try
            {
                var randomAccessStream = await file.OpenReadAsync();
                var readStream = randomAccessStream.AsStreamForRead();
                byte[] fileBytes = new byte[readStream.Length];
                await readStream.ReadAsync(fileBytes,0,fileBytes.Length);
                
                StringBuilder serializedBytes = new StringBuilder();
                fileBytes.ToList().ForEach(x => serializedBytes.AppendFormat("{0}.", Convert.ToUInt32(x)));
                string postParameters = String.Format("fileBytes={0}&fileName={1}", serializedBytes.ToString(), file.Name);
                byte[] postData = Encoding.UTF8.GetBytes(postParameters);

                HttpWebRequest req = CreateWebRequest("/Detection/MobileDetectMark");
                var stream = await req.GetRequestStreamAsync();
                await stream.WriteAsync(postData, 0, postData.Length);
                stream.Dispose();

                HttpWebResponse result = (HttpWebResponse) await req.GetResponseAsync();
                if (result.StatusCode == HttpStatusCode.OK && result.Cookies["user"] != null  && result.Cookies["imageNumber"] != null)
                {
                    PageData.Instance().SetDetectionResultsPageData(result.Cookies["user"].Value, int.Parse(result.Cookies["imageNumber"].Value));
                    success = true;
                }
            }
            catch (Exception E)
            {
                Popup errorPopup = new Popup();

            }
            return success;
        }

        public async static Task<bool> LogIn(string username, string password)
        {
            bool success = false;
            
            try
            {
                StringBuilder serializedBytes = new StringBuilder();
                string postParameters = "email=" + username + "&password=" + password;
                byte[] postData = Encoding.UTF8.GetBytes(postParameters);

                HttpWebRequest request = CreateWebRequest("/Auth/MobileLogIn");
                var stream = await request.GetRequestStreamAsync();
                await stream.WriteAsync(postData, 0, postData.Length);
                stream.Dispose();

                HttpWebResponse result = (HttpWebResponse) await request.GetResponseAsync();
                if (result.StatusCode == HttpStatusCode.OK && result.Cookies[".ASPXAUTH"] != null)
                {
                    authCookie = result.Cookies[".ASPXAUTH"];
                    string authTokenFilePath = ApplicationData.Current.LocalFolder.Path + @"\authToken.txt";
                    File.WriteAllText(authTokenFilePath, authCookie.Value);
                    success = true;
                }
            }
            catch(Exception E)
            {

            }
            
            return success;
        }

        private static HttpWebRequest CreateWebRequest(string path, bool addAuthCookie = false)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = serverHost;
            //builder.Port = 51916;
            builder.Path = path;
            Uri uri = builder.Uri;

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();
            if (addAuthCookie)
            {
                authCookie.Domain = localHost;
                request.CookieContainer.Add(uri, authCookie);
            }
            
            return request;
        }
    }
}
