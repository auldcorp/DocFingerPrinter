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

namespace DocFingerPrinterBeta.DataLayer
{

    public class DataPortal
    {
        private ApplicationDbContext _dbContext;

        public DataPortal()
        {
            _dbContext = new ApplicationDbContext();
        }

        public ResultStatus FileUpload(string imagePath, HttpPostedFileBase file, string imageName, int radio)
        {
            var result = OpenStego.EmbedData("Test embedding of string", imagePath, "C:\\Users\\Public\\test.png");
            OpenStego.WatermarkImage(radio, "test", imagePath, "C:\\Users\\Public\\test.png");

            //get marked image binary
            var markedDrawingImage = System.Drawing.Image.FromFile("C:\\Users\\Public\\test.png");
            byte[] markedImageBinary = ImageToByte(markedDrawingImage);

            //create, initialize new markedImage then save to db
            Models.Image markedImage = new Models.Image();
            markedImage.imageBinary = markedImageBinary;
            markedImage.filename = imageName;
            markedImage.UserId = HttpContext.Current.User.Identity.GetUserId<int>();
            var currentUserId = HttpContext.Current.User.Identity.GetUserId<int>();
            var currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
            markedImage.UniqueMark = currentUser.Id + "#" + currentUser.NumberOfImagesMarked;
            currentUser.NumberOfImagesMarked++;

            //create, initialize new original Image then save to db
            using (MemoryStream ms2 = new MemoryStream())
            {
                Models.Image newImage = new Models.Image();
                file.InputStream.CopyTo(ms2);
                byte[] array = ms2.GetBuffer();
                newImage.imageBinary = array;
                newImage.filename = imageName;
                newImage.UserId = HttpContext.Current.User.Identity.GetUserId<int>();
                currentUserId = HttpContext.Current.User.Identity.GetUserId<int>();
                currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
                currentUser.NumberOfImagesMarked++;

                try
                {
                    _dbContext.Image.Add(markedImage);
                    _dbContext.Image.Add(newImage);
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }

            return result;
        }

        public DetectionResponse DetectSignature(string imagePath)
        {
            DetectionResponse result = new DetectionResponse();
            string imageText = TesseractDetection.getText(imagePath);
            if (!String.IsNullOrEmpty(imageText))
            {
                string imageSignature = TesseractDetection.convertFullMarkToString(imageText);
                if (imageSignature.Length > 1)
                {
                    Models.Image markedImage = _dbContext.Image.Where(x => x.UniqueMark.Equals(imageSignature)).FirstOrDefault();
                    if (markedImage != null)
                    {
                        result.UserName = markedImage.User.UserName;
                        int imageNo;
                        bool parseSuccess = int.TryParse(imageSignature.Substring(imageSignature.IndexOf('#') + 1), out imageNo);
                        if (parseSuccess)
                        {
                            result.ImageNumber = imageNo;
                            result.Status = ResultStatus.Success;
                        }
                    }
                }

            }
            else
            {
                result.Status = ResultStatus.Error;
            }
            return result;
        }

        public List<User> GetUsers()
        {
            List<User> users = _dbContext.Users.ToList();
            return users;
        }

        public List<User> GetUser(int id)
        {
            var user = _dbContext.Users.Find(id);
            List<User> users = new List<User>();
            users.Add(user);
            return users;
        }

        public List<byte[]> GetUserImages(int userId)
        {
            List<System.Drawing.Image> actualImages = new List<System.Drawing.Image>();
            var imagesAsBinary = _dbContext.Image.Where(x => x.UserId == userId).Select(x => x.imageBinary).ToList();
            return imagesAsBinary;
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

    }
}
