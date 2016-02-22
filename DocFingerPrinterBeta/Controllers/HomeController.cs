using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var proc1 = new ProcessStartInfo();
            string anyCommand = "mkdir test";
            proc1.UseShellExecute = true;

            proc1.WorkingDirectory = @"C:\Users\Public";

            proc1.FileName = @"C:\Windows\System32\cmd.exe";

            proc1.Arguments = "/c " + anyCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1);

            return View();
        }
        
    }
}
