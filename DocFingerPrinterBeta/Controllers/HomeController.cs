﻿using System;
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


            string filePath2 = "/c mkdir test2";

            string ExePath2 = "cmd";


            Process myProcess = new Process();
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(ExePath2);

            myProcessStartInfo.Arguments = filePath2;//"C:\\survival_analysis_UAT.pl";
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.RedirectStandardError = true;
            myProcessStartInfo.RedirectStandardInput = true;


            myProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcessStartInfo.CreateNoWindow = true;
            myProcess.StartInfo = myProcessStartInfo;
            myProcess.Start();

            return View();
        }
        
    }
}
