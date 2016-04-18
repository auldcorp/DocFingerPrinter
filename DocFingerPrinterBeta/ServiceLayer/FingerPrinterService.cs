using System;
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
        /// <param name="imagePath"></param>
        /// <param name="fileBytes"></param>
        /// <param name="imageName"></param>
        /// <param name="radio"></param>
        /// <returns>appropriate response based on whether an error occured or not</returns>
        public FileUploadResponse FileUpload(string imagePath, byte[] fileBytes, string imageName, int radio)
        {
            try
            {
                return _dataPortal.FileUpload(imagePath, fileBytes, imageName, radio);       
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
        /// <param name="imagePath"></param>
        /// <returns>appropriate response based on whether an error occured or not</returns>
        public DetectionResponse DetectSignature(string imagePath)
        {
            try
            {
                return _dataPortal.DetectSignature(imagePath);
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
