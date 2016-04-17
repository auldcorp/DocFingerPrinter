using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.ViewModels
{
    /// <summary>
    /// view model for file upload screen
    /// </summary>
    public class FileUploadViewModel
    {
        public string errorMessage { get; set; }
        public string ShowErrorMessage(string message)
        {
            if (message != null)
                return "display:normal";
            return "display:none";
        }
    }
}