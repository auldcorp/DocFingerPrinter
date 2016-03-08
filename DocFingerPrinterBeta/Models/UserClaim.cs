using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DocFingerPrinterBeta.Models
{
    public class UserClaim : IdentityUserClaim<int>
    {
    }
}
