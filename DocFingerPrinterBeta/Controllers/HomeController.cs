using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    /// <summary>
    /// controller that takes user to home page
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// home page
        /// </summary>
        /// <returns>home page</returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
