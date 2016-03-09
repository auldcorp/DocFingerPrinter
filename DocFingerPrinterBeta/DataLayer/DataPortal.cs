﻿using DocFingerPrinterBeta.DAL;
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

namespace DocFingerPrinterBeta.DataLayer
{

    public class DataPortal
    {
        private ApplicationDbContext _dbContext;

        public DataPortal()
        {
            _dbContext = new ApplicationDbContext();
        }

        public ResultStatus FileUpload(string imagePath, HttpPostedFileBase file, string imageName)
        {
            var result = OpenStego.EmbedData("Test embedding of string", imagePath, "C:\\Users\\Public\\test.png");

            using (MemoryStream ms = new MemoryStream())
            {
                Image newImage = new Image();
                file.InputStream.CopyTo(ms);
                byte[] array = ms.GetBuffer();
                newImage.imageBinary = array;
                newImage.filename = imageName;

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
    }
}