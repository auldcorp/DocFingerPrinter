using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPDocFingerPrinter
{
    public static class HttpRequest
    {
        private static string username = "";
        private static string password = "";
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

            HttpWebResponse result = (HttpWebResponse) await req.GetResponseAsync();
            
            if (result.ContentType.Substring(0, 5).Equals("image"))
            {
                var responseStream = result.GetResponseStream();
                byte[] byteResponse = new byte[result.ContentLength];
                responseStream.Read(byteResponse, 0, byteResponse.Length);
                responseStream.Dispose();
    
                await ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name);
                StorageFile markedImageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(file.Name);
                Stream writeToFile = await markedImageFile.OpenStreamForWriteAsync();
                await writeToFile.WriteAsync(byteResponse, 0, byteResponse.Length);
                writeToFile.Dispose();
                return true;
            }

            
            return false;
        }

        public async static Task<bool> LogIn(string user, string pass)
        {
            username = user;
            password = pass;
            var myClient = new HttpClient();
            //var myRequest = new HttpRequestMessage(HttpMethod.Get, "http://docfingerprint.cloudapp.net/Auth/LogIn");
 
            //MultipartContent content = new MultipartContent();
            //StringContent paramUsername = new StringContent(username);
            //StringContent paramPassword = new StringContent(password);
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "docfingerprint.cloudapp.net";
            builder.Path = "/Auth/MobileLogIn";
            builder.Query = "email=" + username + "&password=" + password;
            
            Uri uri = builder.Uri;
            //content.Add(paramUsername);
            //content.Add(paramPassword);
            HttpResponseMessage response = await myClient.PostAsync(uri.ToString(), null);
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Equals("true");
        }
    }
}
