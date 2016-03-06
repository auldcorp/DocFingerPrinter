using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.DAL;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartup("DocFingerPrinterConfig", typeof(DocFingerPrinterBeta.Startup))]

namespace DocFingerPrinterBeta
{
    public partial class Startup
    {
        public static Func<UserManager<User, int>> UserManagerFactory { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });

            // configure the user manager
            UserManagerFactory = () =>
            {
                var usermanager = new UserManager<User, int>(
                    new UserStore<User, Role, int, UserLogin, UserRole, UserClaim>(new ApplicationDbContext()));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<User, int>(usermanager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };

                return usermanager;
            };
        }
    }
}
