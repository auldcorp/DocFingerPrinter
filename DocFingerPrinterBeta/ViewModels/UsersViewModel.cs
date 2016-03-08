using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.ViewModels
{
    public class UsersViewModel
    {
        public List<User> Users { get; set; }
        public string ErrorMessage { get; set; }
    }
}