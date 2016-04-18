using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.Models;

namespace DocFingerPrinterBeta.Controllers
{
    /// <summary>
    /// controller that takes user to home page
    /// </summary>
    public class HomeController : Controller
    {

        private FingerPrinterService _fps = new FingerPrinterService();

        /// <summary>
        /// home page
        /// </summary>
        /// <returns>home page</returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new UsersViewModel();
            ViewBag.Title = "Home Page";
            UsersResponse response = _fps.GetUsers();
            model.Users = response.Users;
            int users = 0;
            int images = 0;
            if (model.Users != null)
            {
                foreach (User user in model.Users)
                {
                    if (user != null)
                    {
                        users++;
                        images += user.NumberOfImagesMarked;
                    }
                }
            }
            ViewBag.Users = users;
            ViewBag.Images = images;
            return View();
        }
    }
}
