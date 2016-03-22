using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}