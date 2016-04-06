using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.Static_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;

namespace DocFingerPrinterBeta.Controllers
{
    public class ImageUploadController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();
        // GET: ImageUpload
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult FileUpload(HttpPostedFileBase file, int radio)
        {
            if (file != null)
            {
                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                file.SaveAs(imagePath);

                FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, file, imageName, radio);
                if (fileUploadResponse.Status == ResultStatus.Error)
                {
                    //do error handling here 
                }
                else
                {
                    var imageData = _fps.GetImageById(fileUploadResponse.SignedImageId).ImageBinary;
                    return File(imageData, "image/png");
                }
            }

            return View("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult MobileFileUpload(HttpPostedFileBase file, int radio)
        {
            if (file != null)
            {
                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                file.SaveAs(imagePath);

                FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, file, imageName, radio);
                if (fileUploadResponse.Status == ResultStatus.Error)
                {
                    //do error handling here 
                }
                else
                {
                    var imageData = _fps.GetImageById(fileUploadResponse.SignedImageId).ImageBinary;
                    return File(imageData, "image/png");
                }
            }

            return View("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ImageDisplay(int id)
        {
            var imageData = _fps.GetImageById(id).ImageBinary;
            return File(imageData, "image/png"); // Might need to adjust the content type based on your actual image type
        }

    }
}