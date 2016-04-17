using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.ViewModels
{
    /// <summary>
    /// view model for the user info screen
    /// </summary>
    public class UserInfoViewModel
    {
        public List<byte[]> Images { get; set; }
        public Models.User User { get; set; }
    }
}