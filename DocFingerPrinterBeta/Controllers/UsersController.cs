using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            ViewBag.Title = "Users Page";
            var m = "s";
            return View();
        }
    }
}