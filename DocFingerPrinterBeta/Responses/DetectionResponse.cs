using System;
using System.Web;

namespace DocFingerPrinterBeta.Responses
{
    /// <summary>
    /// detection response class for mark detections
    /// </summary>
    public class DetectionResponse : BaseResponse
    {
        public string UserName { get; set; }
        public int ImageNumber { get; set; }
    }
}
