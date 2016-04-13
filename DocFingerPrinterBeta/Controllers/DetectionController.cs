using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
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
            var model = new FileUploadViewModel();
            ViewBag.Hidden = "display: none";
            ViewBag.UserName = "";
            ViewBag.ImageNumber = "";
            return View(model);
        }

        public ActionResult DetectMark(HttpPostedFileBase file)
        {
            if (file == null || !file.ContentType.Contains("image"))
            {
                var model = new FileUploadViewModel();
                model.errorMessage = "You must select a jpeg or png file to upload";
                ViewBag.Hidden = "display: none";
                ViewBag.UserName = "";
                ViewBag.ImageNumber = "";
                return View("Index", model);
            }
            DetectionResponse response = _fps.DetectSignature(file.FileName);
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