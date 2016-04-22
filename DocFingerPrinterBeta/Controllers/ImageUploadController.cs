using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.ServiceLayer;
using DocFingerPrinterBeta.Static_Classes;
using DocFingerPrinterBeta.ViewModels;
using Microsoft.AspNet.Identity;
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

    /// <summary>
    /// controller that handles image uploading/marking
    /// </summary>
    public class ImageUploadController : Controller
    {
        private FingerPrinterService _fps = new FingerPrinterService();
        private readonly UserManager<User, int> userManager;

        public ImageUploadController()
            : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public ImageUploadController(UserManager<User, int> userManager)
        {
            this.userManager = userManager;
        }

        // GET: ImageUpload
        /// <summary>
        /// image upload page
        /// </summary>
        /// <returns>image upload page</returns>
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            var model = new FileUploadViewModel();
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View(model);
        }

        /// <summary>
        /// uploads file and marks file during FileUpload
        /// </summary>
        /// <param name="file"></param>
        /// <param name="radio"></param>
        /// <returns>page of uploaded image unless error occurs</returns>
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult FileUpload(HttpPostedFileBase file, int radio, bool checkbox)
        {
            Debug.Print("checkbox: " + checkbox.ToString());
            var model = new FileUploadViewModel();
            if (file != null)
            {
                var fileContentType = file.ContentType;
                if (!fileContentType.Contains("image"))
                {
                    model.errorMessage = "You must select a jpeg or png file to upload";
                    ViewBag.Title = "Upload Page";
                    ViewBag.Link = "";
                    ViewBag.Hidden = "display: none";
                    return View("Index", model);
                }

                string imageName = Path.GetFileName(file.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/images/profile"), imageName);
                file.SaveAs(imagePath);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] fileArray = ms.GetBuffer();
                    ms.Close();
                    file.InputStream.Dispose();

                    FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, fileArray, imageName, radio, checkbox);
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
                System.IO.File.Delete(imagePath);
            }

            model = new FileUploadViewModel();
            model.errorMessage = "You must select a jpeg or png file to upload";
            ViewBag.Title = "Upload Page";
            ViewBag.Link = "";
            ViewBag.Hidden = "display: none";
            return View("Index", model);
        }

        /// <summary>
        /// same as FileUpload except for mobile
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="radio"></param>
        /// <returns>json containing image file uploaded</returns>
        [HttpPost]
        public async Task<HttpResponseBase> MobileFileUpload(string fileBytes, string fileName, int radio)
        {
            HttpCookie authCookie = null;
            Models.User user = null;
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                try
                {
                    authCookie = Request.Cookies[".ASPXAUTH"];
                    UsersResponse response = _fps.GetUserFromAuthToken(authCookie.Value);
                    if (response.Status == ResultStatus.Success && response.Users.Count() > 0)
                        user = _fps.GetUserFromAuthToken(authCookie.Value).Users.First();
                }
                catch (Exception E)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.StatusDescription = E.Message;
                    return Response;
                }
            }
                
            if (user != null && !string.IsNullOrEmpty(fileBytes))
            {
                try
                {
                    var identity = await userManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ExternalCookie);

                    Request.GetOwinContext().Authentication.SignIn(identity);
    
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

                    FileUploadResponse fileUploadResponse = _fps.FileUpload(imagePath, imageBytes, fileName, radio, false);
                    if (fileUploadResponse.Status == ResultStatus.Error)
                    {
                        Response.StatusCode = (int)fileUploadResponse.Status;
                        Response.StatusDescription = fileUploadResponse.Message;
                        return Response;
                    }
                    else
                    {
                        var imageData = _fps.GetImageById(fileUploadResponse.SignedImageId).ImageBinary;
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        Response.ContentType = "image/png";
                        Response.Buffer = true;
                        Stream responseStream = Response.OutputStream;
                        responseStream.Write(imageData, 0, imageData.Length);
                        responseStream.Flush();
                        Response.Flush(); 
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
            Response.StatusDescription = "Error: Problem with authorization or null file string";
            return Response;
        }

        /// <summary>
        /// displays image based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>page of image with id param</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ImageDisplay(int id)
        {
            var imageData = _fps.GetImageById(id).ImageBinary;
            return File(imageData, "image/png"); // Might need to adjust the content type based on your actual image type
        }

    }
}