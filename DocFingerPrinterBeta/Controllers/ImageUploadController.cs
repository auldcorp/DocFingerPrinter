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

        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                file.SaveAs(imagePath);

                string openstegoPath = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";
                string secretTextPath = Path.Combine(Server.MapPath("~/texts"), "secretText.txt");


                var proc1 = new ProcessStartInfo();
                string embedCommand = "java -jar " +openstegoPath + " embed -a RandomLSB -mf \"" +secretTextPath + "\" -cf \"" +path + "\" -sf \"C:\\Users\\Public\\test.png\"";
                proc1.UseShellExecute = true;

                proc1.WorkingDirectory = @"C:\Users\Public";

                proc1.FileName = @"C:\Windows\System32\cmd.exe";

                proc1.Arguments = "/c " + embedCommand;
                proc1.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(proc1);
                Image newImage = new Image();

                using (MemoryStream ms = new MemoryStream())
                {
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
                    //do error handling here
                    }
                }


            ViewBag.Link = "C:\\Users\\Public\\test.png";
            ViewBag.Hidden = "";

            return View("Index");
        }
    }
}