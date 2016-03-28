using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Responses
{
    public class FileUploadResponse : BaseResponse
    {
        public int SignedImageId { get; set; }
    }
}