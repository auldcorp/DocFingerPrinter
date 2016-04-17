using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DocFingerPrinterBeta.Controllers
{
    /// <summary>
    /// controller that handles user information
    /// </summary>
    public class UsersController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();
        // GET: Users

        /// <summary>
        /// list of users page
        /// </summary>
        /// <returns>page with a list of users</returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new UsersViewModel();
            ViewBag.Title = "Users Page";
            UsersResponse response = _fps.GetUsers();
            model.Users = response.Users;

            return View(model);
        }

        /// <summary>
        /// page of logged in user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>page with userInfo for currently logged in user</returns>
        [Authorize(Roles = "User, Admin")]
        public ActionResult UserInfo(int id)
        {
            var model = new UserInfoViewModel();
            model.User = _fps.GetUser(id).Users.FirstOrDefault();
            model.Images = _fps.GetUserImages(id).Images;

            return View(model);
        }
    }
}