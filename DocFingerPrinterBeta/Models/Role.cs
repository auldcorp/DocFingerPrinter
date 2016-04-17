using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Models
{
    /// <summary>
    /// role model
    /// </summary>
    public class Role : IdentityRole<int, UserRole>
    {
        public string Description { get; set; }
    }
}