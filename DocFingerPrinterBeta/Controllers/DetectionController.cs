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
    /// <summary>
    /// controller that handles mark detection
    /// </summary>
    public class DetectionController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();

        // GET: Detection
        /// <summary>
        /// image detection home page
        /// </summary>
        /// <returns>image detection page</returns>
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            var model = new FileUploadViewModel();
            ViewBag.Hidden = "display: none";
            ViewBag.UserName = "";
            ViewBag.ImageNumber = "";
            return View(model);
        }

        /// <summary>
        /// reads in image file and runs DetectSignature on that file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>page displaying who uploaded file belongs to</returns>
        public ActionResult DetectMark(HttpPostedFileBase file)
        {
            var model = new FileUploadViewModel();
            //validation on file input
            if (file == null || !file.ContentType.Contains("image"))
            {
                model.errorMessage = "You must select a png file to upload";
                ViewBag.Hidden = "display: none";
                ViewBag.UserName = "";
                ViewBag.ImageNumber = "";
                return View("Index", model);
            }
            string imageName = Path.GetFileName(file.FileName);
            string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);
            file.SaveAs(imagePath);
            DetectionResponse response = _fps.DetectSignature(imagePath);
            file.InputStream.Dispose();
            System.IO.File.Delete(imagePath);
            //if sig detection success return user/image id else return error
            if (response.Status == ResultStatus.Success)
            {
                model.errorMessage = null;
                if (response.UserName != null)
                    ViewBag.UserName = response.UserName;

                ViewBag.ImageNumber = response.ImageNumber;
                ViewBag.Hidden = "";
                return View("Index", model);
            }
            else
            {
                model.errorMessage = response.Message;
                ViewBag.Hidden = "display: none";
                ViewBag.UserName = "";
                ViewBag.ImageNumber = "";
                return View("Index", model);
            }
        }
    }
}