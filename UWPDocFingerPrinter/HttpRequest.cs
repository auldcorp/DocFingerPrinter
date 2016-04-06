using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace UWPDocFingerPrinter
{
    public static class HttpRequest
    {
        public async static Task<bool> UploadFile(StorageFile file, int corner)
        {
            var myClient = new HttpClient();
            var myRequest = new HttpRequestMessage(HttpMethod.Get, "http://docfingerprint.cloudapp.net/ImageUpload");
            //myRequest.Content = file;
            var response = await myClient.SendAsync(myRequest, HttpCompletionOption.ResponseContentRead);
            return true;
        }

        public async static Task<bool> LogIn(string username, string password)
        {
            var myClient = new HttpClient();
            //var myRequest = new HttpRequestMessage(HttpMethod.Get, "http://docfingerprint.cloudapp.net/Auth/LogIn");
            var myRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:51916/Auth/MobileLogIn");
 
             MultipartContent content = new MultipartContent();
            StringContent paramUsername = new StringContent(username);
            StringContent paramPassword = new StringContent(password);
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "docfingerprint.cloudapp.net";
            builder.Path = "/Auth/MobileLogIn";
            builder.Query = "email=" + username + "&password=" + password;
            
            Uri uri = builder.Uri;
            content.Add(paramUsername);
            content.Add(paramPassword);
            myRequest.Content = content;
            HttpResponseMessage response = await myClient.PostAsync(uri.ToString(), content);
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Equals("true");
        }
    }
}
