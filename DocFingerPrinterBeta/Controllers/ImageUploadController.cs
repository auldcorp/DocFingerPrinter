using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.Static_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View();
        }

        [HttpGet]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            Tesseract.TesseractEngine test = new Tesseract.TesseractEngine("falseDataPath", "eng");
            if (file != null)
            {
                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                file.SaveAs(imagePath);

                BaseResponse fileUploadResponse = _fps.FileUpload(imagePath, file, imageName);
                if (fileUploadResponse.Status == ResultStatus.Error)
                {
                    //do error handling here 
                }
            }

            ViewBag.Link = "C:\\Users\\Public\\test.png";
            ViewBag.Hidden = "";

            return View("Index");
        }
    }
}