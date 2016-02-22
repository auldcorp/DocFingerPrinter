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
                                       Server.MapPath("~/images/profile"), image);
                
                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }

            //file has been uploaded now do opensteg on image to mark it
            //then redirect back to where ever
            return RedirectToAction("Index", "ImageUpload");
        }
    }
}