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

                string openstegoPath = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";
                string secretTextPath = Path.Combine(Server.MapPath("~/texts"), "secretText.txt");
                string embedCommand = "java -jar " +openstegoPath + " embed -a RandomLSB -mf \"" +secretTextPath + "\" -cf \"" +path + "\" -sf \"C:\\Users\\Public\\test.png\"";
                string workingDirectory = @"C:\Users\Public";

                var result = CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);

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