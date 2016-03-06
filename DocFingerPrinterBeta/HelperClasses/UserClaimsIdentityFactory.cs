using DocFingerPrinterBeta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.HelperClasses
{
    public class UserClaimsIdentityFactory : ClaimsIdentityFactory<User, int>
    {
        public override async Task<ClaimsIdentity> CreateAsync(UserManager<User, int> manager, User user, string authenticationType)
        {
            var identity = await base.CreateAsync(manager, user, authenticationType);

            return identity;
        }
    }
}
