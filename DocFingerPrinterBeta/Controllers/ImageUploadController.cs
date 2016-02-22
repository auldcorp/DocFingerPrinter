using DocFingerPrinterBeta.Models;
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
            return View();
        }


        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string image = Path.GetFileName(file.FileName);
                string path = Path.Combine(
                                       Server.MapPath("~/images/profile"), image);

                file.SaveAs(path);

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
                        Console.Write(e);
                    }
                }

                //Show user encoded file
                return base.File("C:\\Users\\Public\\test.png", "image/png");
            }

            //file has been uploaded now do opensteg on image to mark it
            //then redirect back to where ever
            return RedirectToAction("Index", "ImageUpload");
        }
    }
}