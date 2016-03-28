using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    public class DetectionController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();

        // GET: Detection
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            ViewBag.Hidden = "display: none";
            ViewBag.UserName = "";
            ViewBag.ImageNumber = "";
            return View();
        }

        public ActionResult DetectMark(HttpPostedFileBase file)
        {
            string imagePath = Path.Combine(Server.MapPath("~/images/profile"), Path.GetFileName(file.FileName));
            DetectionResponse response = _fps.DetectSignature(imagePath);
            if(response.Status == ResultStatus.Success)
            {
                if (response.UserName != null)
                    ViewBag.UserName = response.UserName;
                ViewBag.ImageNumber = response.ImageNumber;
                ViewBag.Hidden = "";
            }
            return View("Index");
        }
    }
}