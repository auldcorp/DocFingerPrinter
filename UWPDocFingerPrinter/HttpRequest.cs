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
        public async static Task<bool> UploadFile(StorageFile file)
        {
            var myClient = new HttpClient();
            var myRequest = new HttpRequestMessage(HttpMethod.Get, "http://docfingerprint.cloudapp.net/api/values");
            var response = await myClient.SendAsync(myRequest);
            return true;
        }
    }
}
