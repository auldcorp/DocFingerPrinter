using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Providers;
using DocFingerPrinterBeta.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DocFingerPrinterBeta.Controllers
{
    /// <summary>
    /// controller that handles user registration and authentication
    /// </summary>
    public class AuthController : Controller
    {
        private const string ApiUri = "http://docfingerprint.cloudapp.net/";
        private readonly UserManager<User, int> userManager;

        public AuthController()
            : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public AuthController(UserManager<User, int> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// login page/route
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>login page</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new LogInModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        /// <summary>
        /// takes in the login model data and validates/authenticates log in
        /// </summary>
        /// <param name="model"></param>
        /// <returns>the returnURL page</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindAsync(model.Email, model.Password);

            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ApplicationCookie);

                GetAuthenticationManager().SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }


        /// <summary>
        /// takes email/password for login and validates/authenticates mobile login
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<HttpResponseBase> MobileLogIn(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusDescription = "Invalid Model";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                return Response;
            }

            var user = await userManager.FindAsync(email, password);

            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ExternalCookie);

                GetAuthenticationManager().SignIn(identity);
                
                Response.StatusCode = (int)HttpStatusCode.OK;
                string encTicket;
                if (string.IsNullOrEmpty(user.AuthTokenValue))
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, email, DateTime.Now, DateTime.MaxValue, true, string.Empty);
                    encTicket = FormsAuthentication.Encrypt(ticket);
                    user.AuthTokenValue = encTicket;
                    var identityResult = await userManager.UpdateAsync(user);
                }
                else
                {
                    encTicket = user.AuthTokenValue;
                }
                
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Path = FormsAuthentication.FormsCookiePath };
                Response.AppendCookie(authCookie);

                return Response;
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Response;

        }

        /// <summary>
        /// logs out user
        /// </summary>
        /// <returns>logout screen</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            var authManager = GetAuthenticationManager();
            authManager.SignOut();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            return View();
        }

        /// <summary>
        /// route for register page
        /// </summary>
        /// <returns>register page</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// takes in registartion model data and registers/creates new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>home page unless error occured</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new User
            {
                UserName = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var currentUser = userManager.FindByName(user.UserName);
                var roleResult = userManager.AddToRole(currentUser.Id, "User");

                await SignIn(user);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }


        /// <summary>
        /// signs user into Identity framework
        /// </summary>
        /// <param name="user"></param>
        /// <returns>void</returns>
        [AllowAnonymous]
        [HttpPost]
        private async Task SignIn(User user)
        {
            var identity = await userManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);

            GetAuthenticationManager().SignIn(identity);
        }

        /// <summary>
        /// gets redirctURL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>redirect url</returns>
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        /// <summary>
        /// gets the authentication manager from the OWIN context
        /// </summary>
        /// <returns></returns>
        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }


        /// <summary>
        /// disposes of userManager object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}