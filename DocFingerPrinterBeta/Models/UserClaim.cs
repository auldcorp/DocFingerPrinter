using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DocFingerPrinterBeta.Models
{
    /// <summary>
    /// claim model
    /// </summary>
    public class UserClaim : IdentityUserClaim<int>
    {
    }
}
