using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.Static_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Drawing;
using System.Data.Entity;
using System.Drawing.Imaging;

namespace DocFingerPrinterBeta.DataLayer
{
    /// <summary>
    /// class that handles DB crud operations
    /// </summary>
    public class DataPortal
    {
        private ApplicationDbContext _dbContext;

        public DataPortal()
        {
            _dbContext = new ApplicationDbContext();
        }

        /// <summary>
        /// takes in an image marks that image and saves it the DB
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="fileBytes"></param>
        /// <param name="imageName"></param>
        /// <param name="radio"></param>
        /// <returns>id of uploaded image</returns>
        public FileUploadResponse FileUpload(string imagePath, byte[] fileBytes, string imageName, int radio)
        {
            FileUploadResponse response = new FileUploadResponse();
            var currentUserId = HttpContext.Current.User.Identity.GetUserId<int>();
            var currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
            currentUser.NumberOfImagesMarked++;


            //create, initialize new markedImage then save to db
            Models.Image markedImage = new Models.Image();
            markedImage.OriginalImageBinary = fileBytes;
            markedImage.Filename = imageName;
            markedImage.UserId = HttpContext.Current.User.Identity.GetUserId<int>();
            markedImage.User = currentUser;
            markedImage.UniqueMark = currentUser.Id + "#" + currentUser.NumberOfImagesMarked;

            response.Status = OpenStego.EmbedData(markedImage.UniqueMark, imagePath, "C:\\Users\\Public\\test.png");
            string signature = "\\" +TesseractDetection.convertIntToBinarySignature(currentUser.Id) + "#" + TesseractDetection.convertIntToBinarySignature(currentUser.NumberOfImagesMarked) +"#";
            byte[] markedImageBinary = OpenStego.WatermarkImage(radio, signature, imagePath);
            markedImage.MarkedImageBinary = markedImageBinary;
            response.Status = ResultStatus.Success;            

            try
            {
                _dbContext.Image.Add(markedImage);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.Write(e);
                response.Status = ResultStatus.Error;
                response.Message = e.ToString();
            }
            
            response.SignedImageId = markedImage.Id;

            return response;
        }

        /// <summary>
        /// takes in an image and looks to see if it is marked, if it is returns the user asscoiated with that image
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns>username and image number</returns>
        public DetectionResponse DetectSignature(string imagePath)
        {
            DetectionResponse result1 = new DetectionResponse();
            DetectionResponse result2 = new DetectionResponse();
            string extractText = OpenStego.ExtractDataFromFile(imagePath);
            string imageText = TesseractDetection.getText(imagePath);
            if (!String.IsNullOrEmpty(extractText))
            {
                Models.Image markedImage = _dbContext.Image.Where(x => x.UniqueMark.Equals(extractText)).Include("User").FirstOrDefault();
                if (markedImage != null)
                {
                    result2.UserName = markedImage.User.UserName;
                    int imageNo;
                    bool parseSuccess = int.TryParse(extractText.Substring(extractText.IndexOf('#') + 1), out imageNo);
                    if (parseSuccess)
                    {
                        result2.ImageNumber = imageNo;
                        result2.Status = ResultStatus.Success;
                    }
                }
                else
                {
                    result2.Status = ResultStatus.Error;
                }
            }
            else if (!String.IsNullOrEmpty(imageText))
            {
                imageText = TesseractDetection.removeNewLineCharacters(imageText);
                imageText = TesseractDetection.removeWhiteSpaces(imageText);
                string imageSignature = TesseractDetection.convertFullMarkToString(imageText);
                if (imageSignature.Length > 1)
                {
                    Models.Image markedImage = _dbContext.Image.Where(x => x.UniqueMark.Equals(imageSignature)).Include("User").FirstOrDefault();
                    if (markedImage != null)
                    {
                        result2.UserName = markedImage.User.UserName;
                        int imageNo;
                        bool parseSuccess = int.TryParse(imageSignature.Substring(imageSignature.IndexOf('#') + 1), out imageNo);
                        if (parseSuccess)
                        {
                            result2.ImageNumber = imageNo;
                            result2.Status = ResultStatus.Success;
                        }
                    }
                    else
                    {
                        result2.Status = ResultStatus.Error;
                    }
                }
            }
            else
            {
                result2.Status = ResultStatus.Error;
            }
            return result2;
        }

        /// <summary>
        /// gets list of users registered in the DB
        /// </summary>
        /// <returns>list of registered users</returns>
        public List<User> GetUsers()
        {
            List<User> users = _dbContext.Users.ToList();
            return users;
        }

        /// <summary>
        /// gets user with param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returns users with param id</returns>
        public List<User> GetUser(int id)
        {
            var user = _dbContext.Users.Find(id);
            List<User> users = new List<User>();
            users.Add(user);
            return users;
        }

        /// <summary>
        /// gets images associated with user with userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of images associated with certain user</returns>
        public List<byte[]> GetUserImages(int userId)
        {
            List<System.Drawing.Image> actualImages = new List<System.Drawing.Image>();
            var imagesAsBinary = _dbContext.Image.Where(x => x.UserId == userId).Select(x => x.MarkedImageBinary).ToList();
            return imagesAsBinary;
        }

        /// <summary>
        /// gets image with param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>image with associated id</returns>
        public byte[] GetImageById(int id)
        {
            Models.Image image = _dbContext.Image.Where(x => x.Id == id).FirstOrDefault();
            return image.MarkedImageBinary;
        }


    }
}
