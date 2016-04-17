using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.Static_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.HtmlControls;

namespace DocFingerPrinterBeta.Controllers
{
    public class ImageUploadController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();
        // GET: ImageUpload
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult FileUpload(HttpPostedFileBase file, int radio)
        {
            if (file != null)
            {
                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);
                file.SaveAs(imagePath);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] fileArray = ms.GetBuffer();
                    ms.Close();

                    FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, fileArray, imageName, radio);
                    if (fileUploadResponse.Status == ResultStatus.Error)
                    {
                        //do error handling here 
                    }
                    else
                    {
                        var imageData = _fps.GetImageById(fileUploadResponse.SignedImageId).ImageBinary;
                        return File(imageData, "image/png");
                    }
                }
            }

            return View("Index");
        }

        [HttpPost]
        public HttpResponseBase MobileFileUpload(string fileBytes, string fileName, int radio)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (!string.IsNullOrEmpty(fileBytes))
            {
                try
                {
                    string imagePath = Path.Combine(Server.MapPath("~/images/profile"), fileName);

                    string[] byteToConvert = fileBytes.Split('.');
                    List<byte> fileBytesList = new List<byte>();
                    byteToConvert.ToList()
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList()
                            .ForEach(x => fileBytesList.Add(Convert.ToByte(x)));

                    byte[] imageBytes = fileBytesList.ToArray();
                    Bitmap image;
                    using (Stream imageStream = new MemoryStream(imageBytes))
                    {
                        image = new Bitmap(imageStream);
                        imageStream.Close();
                        image.Save(imagePath);
                    }

                    FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, imageBytes, fileName, radio);
                    if (fileUploadResponse.Status == ResultStatus.Error)
                    {
                        Response.StatusDescription = fileUploadResponse.Message;
                        Response.StatusCode = (int)fileUploadResponse.Status;
                        return Response;
                    }
                    else
                    {
                        var imageData = _fps.GetImageById(fileUploadResponse.SignedImageId).ImageBinary;
                        StringBuilder serializedBytes = new StringBuilder();
                        imageData.ToList().ForEach(x => serializedBytes.AppendFormat("{0}.", Convert.ToUInt32(x)));
                        string responseString = serializedBytes.ToString();
                        byte[] responseData = Encoding.UTF8.GetBytes(responseString);
                        Response.ContentEncoding = Encoding.UTF8;
                        Response.ContentType = "image/png";
                        Response.BinaryWrite(responseData);
                        Response.Flush();
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Response;
                    }
                } catch (Exception E)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.StatusDescription = E.Message;
                    return Response;
                }
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            Response.StatusDescription = "Error: File input cannot be null.";
            return Response;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ImageDisplay(int id)
        {
            var imageData = _fps.GetImageById(id).ImageBinary;
            return File(imageData, "image/png"); // Might need to adjust the content type based on your actual image type
        }

    }
}