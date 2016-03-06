using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
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
        private MyDbContext db = new MyDbContext();
        // GET: ImageUpload
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View();
        }

        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string image = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/profile"), image);
                file.SaveAs(path);

                string dataToEmbed = "this is a test by chaz";
                string outputFile = "C:\\Users\\Public\\test.png";
                OpenStego.EmbedData(dataToEmbed, path, outputFile);

                using (MemoryStream ms = new MemoryStream())
                {
                    Image newImage = new Image();
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    newImage.imageBinary = array;
                    newImage.filename = image;
                    try
                    {
                        db.Images.Add(newImage);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }
                }

            }
            ViewBag.Link = "C:\\Users\\Public\\test.png";
            ViewBag.Hidden = "";

            return View("Index");
        }
    }
}