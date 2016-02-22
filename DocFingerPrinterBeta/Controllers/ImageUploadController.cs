using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    public class ImageUploadController : Controller
    {

        private MyDbContext db = new MyDbContext();
        // GET: ImageUpload
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string image = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath(""), image);
                
                file.SaveAs(path);
                Image newImage = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    newImage = new Image();
                    newImage.imageBinary = array;
                    newImage.filename = image;
                    db.SaveChanges();
                }

                
            }
            db.Images.Add

            //file has been uploaded now do opensteg on image to mark it
            //then redirect back to where ever
            return RedirectToAction("Index", "ImageUpload");
        }
    }
}