using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace DocFingerPrinterBeta.Models
{
    /// <summary>
    /// user model
    /// </summary>
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public int NumberOfImagesMarked { get; set; }
        public string AuthTokenValue { get; set; }
    }
}