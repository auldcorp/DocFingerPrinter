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
    public class UsersController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();
        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new UsersViewModel();
            ViewBag.Title = "Users Page";
            UsersResponse response = _fps.GetUsers();
            model.Users = response.Users;

            return View(model);
        }

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