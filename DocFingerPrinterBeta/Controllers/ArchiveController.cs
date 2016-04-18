using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    public class ArchiveController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();

        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            ArchiveViewModel model = new ArchiveViewModel();
            model.Record = _fps.GetAllImageRecords().Images;
            return View(model);
        }
    }
}