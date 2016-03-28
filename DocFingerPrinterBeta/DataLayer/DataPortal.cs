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

            using (MemoryStream ms = new MemoryStream())
            {
                Image newImage = new Image();
                file.InputStream.CopyTo(ms);
                byte[] array = ms.GetBuffer();
                newImage.imageBinary = array;
                newImage.filename = imageName;
                newImage.UserId = HttpContext.Current.User.Identity.GetUserId<int>();
                var currentUserId = HttpContext.Current.User.Identity.GetUserId<int>();
                var currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
                newImage.UniqueMark = currentUser.Id + "#" + currentUser.NumberOfImagesMarked;
                currentUser.NumberOfImagesMarked++;

                try
                {
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
    }
}
