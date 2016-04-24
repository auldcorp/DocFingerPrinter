using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DocFingerPrinterBeta.Controllers
{
    /// <summary>
    /// controller that handles mark detection
    /// </summary>
    public class DetectionController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();

        // GET: Detection
        /// <summary>
        /// image detection home page
        /// </summary>
        /// <returns>image detection page</returns>
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            var model = new FileUploadViewModel();
            ViewBag.Hidden = "display: none";
            ViewBag.UserName = "";
            ViewBag.ImageNumber = "";
            return View(model);
        }

        /// <summary>
        /// reads in image file and runs DetectSignature on that file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>page displaying who uploaded file belongs to</returns>
        public ActionResult DetectMark(HttpPostedFileBase file)
        {
            var model = new FileUploadViewModel();
            //validation on file input
            if (file == null || !file.ContentType.Contains("image"))
            {
                model.errorMessage = "You must select a png file to upload";
                ViewBag.Hidden = "display: none";
                ViewBag.UserName = "";
                ViewBag.ImageNumber = "";
                return View("Index", model);
            }

            MemoryStream ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            byte[] fileBytes = ms.GetBuffer();
            ms.Close();

            DetectionResponse response = _fps.DetectSignature(fileBytes, Path.GetFileName(file.FileName));

            //if sig detection success return user/image id else return error
            if (response.Status == ResultStatus.Success)
            {
                model.errorMessage = null;
                if (response.UserName != null)
                    ViewBag.UserName = response.UserName;

                ViewBag.ImageNumber = response.ImageNumber;
                ViewBag.Hidden = "";
                return View("Index", model);
            }
            else
            {
                model.errorMessage = response.Message;
                ViewBag.Hidden = "display: none";
                ViewBag.UserName = "";
                ViewBag.ImageNumber = "";
                return View("Index", model);
            }
        }

        /// <summary>
        /// same as DetectMark except for mobile
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <returns>HttpResponseBase containing image details if image contains mark</returns>
        [HttpPost]
        public HttpResponseBase MobileDetectMark(string fileBytes, string fileName)
        {
            if (!string.IsNullOrEmpty(fileBytes))
            {
                try
                {
                    string[] byteToConvert = fileBytes.Split('.');
                    List<byte> fileBytesList = new List<byte>();
                    byteToConvert.ToList()
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList()
                            .ForEach(x => fileBytesList.Add(Convert.ToByte(x)));

                    byte[] imageBytes = fileBytesList.ToArray();

                    DetectionResponse detectionResponse = _fps.DetectSignature(imageBytes, fileName);
                    if (detectionResponse.Status == ResultStatus.Error)
                    {
                        Response.StatusCode = (int)detectionResponse.Status;
                        Response.StatusDescription = detectionResponse.Message;
                        return Response;
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        Response.AppendCookie(new HttpCookie("user", detectionResponse.UserName));
                        Response.AppendCookie(new HttpCookie("imageNumber", detectionResponse.ImageNumber.ToString()));
                        return Response;
                    }
                }
                catch (Exception E)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.StatusDescription = E.Message;
                    return Response;
                }
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            Response.StatusDescription = "Error: null file string";
            return Response;
        }
    }
}