using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Responses
{
    /// <summary>
    /// fileUpload response for uploading of images/files
    /// </summary>
    public class FileUploadResponse : BaseResponse
    {
        public int SignedImageId { get; set; }
    }
}