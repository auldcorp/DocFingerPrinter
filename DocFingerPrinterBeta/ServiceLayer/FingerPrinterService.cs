﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.Static_Classes;
using DocFingerPrinterBeta.DAL;
using DocFingerPrinterBeta.DataLayer;
using System.Web;

namespace DocFingerPrinterBeta.ServiceLayer
{
    /// <summary>
    /// service layer to handle returning responses to controllers based on results from data portal
    /// </summary>
    public class FingerPrinterService
    {
        private DataPortal _dataPortal;

        public FingerPrinterService()
        {
             _dataPortal = new DataPortal();
        }

        /// <summary>
        /// calls dataPortal.FileUpload to upload file
        /// </summary>
        /// <param name="userId"
        /// <param name="fileBytes"></param>
        /// <param name="imageName"></param>
        /// <param name="radio"></param>
        /// <param name="transparentSignatureBackground"
        /// <returns>appropriate response based on whether an error occured or not</returns>
        public FileUploadResponse FileUpload(int userId, byte[] fileBytes, string imageName, int radio, bool transparentSignatureBackground)
        {
            try
            {
                return _dataPortal.FileUpload(userId, fileBytes, imageName, radio, transparentSignatureBackground);       
            }
            catch (Exception e)
            {
                return new FileUploadResponse
                {
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        public ImageRecordsResponse GetAllImageRecords()
        {
            try
            {
                return new ImageRecordsResponse
                {
                    Images = _dataPortal.GetAllImageRecords()
                };
            }
            catch (Exception e)
            {
                return new ImageRecordsResponse
                {
                    Images = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.DetectSignature to detect the signature on the image
        /// </summary>
        /// <param name="fileBytes">Byte array of file data</param>
        /// <param name="fileName">Name of file</param>
        /// <returns>appropriate response based on whether an error occured or not</returns>
        public DetectionResponse DetectSignature(byte[] fileBytes, string fileName)
        {
            try
            {
                return _dataPortal.DetectSignature(fileBytes, fileName);
            }
            catch (Exception e)
            {
                return new DetectionResponse
                {
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.GetUsers
        /// </summary>
        /// <returns>users along with appropriate response based on whether an error occured or not</returns>
        public UsersResponse GetUsers()
        {
            try
            {
                return new UsersResponse
                {
                    Users = _dataPortal.GetUsers(),
                    Status = ResultStatus.Success,
                    Message = "success"
                };
            }
            catch (Exception e)
            {
                return new UsersResponse
                {
                    Users = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.GetUserByAuthToken
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns>user along with appropriate response based on whether an error occured or not</returns>
        public UsersResponse GetUserFromAuthToken(string authToken)
        {
            try
            {
                return new UsersResponse
                {
                    Users = _dataPortal.GetUserByAuthToken(authToken),
                    Status = ResultStatus.Success,
                    Message = "success"
                };
            }
            catch (Exception e)
            {
                return new UsersResponse
                {
                    Users = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.GetUser
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>user along with appropriate response based on whether an error occured or not</returns>
        public UsersResponse GetUser(int userId)
        {
            try
            {
                return new UsersResponse
                {
                    Users = _dataPortal.GetUser(userId),
                    Status = ResultStatus.Success,
                    Message = "success"
                };
            }
            catch (Exception e)
            {
                return new UsersResponse
                {
                    Users = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.GetUserImages
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>user images along with appropriate response based on whether an error occured or not</returns>
        public ImagesResponse GetUserImages(int userId)
        {
            try
            {
                return new ImagesResponse
                {
                    Images = _dataPortal.GetUserImages(userId),
                    Status = ResultStatus.Success,
                    Message = "success"
                };
            }
            catch (Exception e)
            {
                return new ImagesResponse
                {
                    Images = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString()
                };
            }
        }

        /// <summary>
        /// calls dataPortal.GetImageById
        /// </summary>
        /// <param name="id"></param>
        /// <returns>specific image along with appropriate response based on whether an error occured or not</returns>
        public ImageBinaryResponse GetImageById(int id)
        {
            try
            {
                return new ImageBinaryResponse
                {
                    ImageBinary = _dataPortal.GetImageById(id),
                    Status = ResultStatus.Success,
                    Message = "success"
                };
            }
            catch(Exception e)
            {
                return new ImageBinaryResponse
                {
                    ImageBinary = null,
                    Status = ResultStatus.Error,
                    Message = e.ToString() 
                };
            }

        }
    }
}
