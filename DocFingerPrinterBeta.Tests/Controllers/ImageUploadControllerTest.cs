using DocFingerPrinterBeta.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Tests.Controllers
{
    [TestClass]
    public class ImageUploadControllerTest
    {
        [TestMethod]
        public void ControllerIndex()
        {
            // Arrange
            ImageUploadController controller = new ImageUploadController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Upload Page", result.ViewBag.Title);
        }

        [TestMethod]
        public void ControllerActionResult()
        {
            // Arrange
            ImageUploadController controller = new ImageUploadController();

            // Act
            ActionResult result = controller.FileUpload(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("System.Web.Mvc.RedirectToRouteResult", result.ToString());
        }
    }
}
