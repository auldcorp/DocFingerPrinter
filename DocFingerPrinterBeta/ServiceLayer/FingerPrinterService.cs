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
    public class FingerPrinterService
    {
        private DataPortal _dataPortal;

        public FingerPrinterService()
        {
             _dataPortal = new DataPortal();
        }

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
