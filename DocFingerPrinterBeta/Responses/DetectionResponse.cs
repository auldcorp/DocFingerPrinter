using System;
using System.Web;

namespace DocFingerPrinterBeta.Responses
{
    public class DetectionResponse : BaseResponse
    {
        public string UserName { get; set; }
        public int ImageNumber { get; set; }
    }
}
