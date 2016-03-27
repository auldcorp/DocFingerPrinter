using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.ViewModels
{
    public class UserInfoViewModel
    {
        public List<byte[]> Images { get; set; }
        public Models.User User { get; set; }
    }
}