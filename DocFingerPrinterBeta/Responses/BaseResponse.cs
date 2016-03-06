using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Responses
{

    public enum ResultStatus { Success, Error }

    public class BaseResponse
    {
        public ResultStatus Status { get; set; }       
        public string Message { get; set; }
    }
}