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
    public class FingerPrinterService
    {
        private DataPortal _dataPortal;

        public FingerPrinterService()
        {
             _dataPortal = new DataPortal();
        }

        public BaseResponse FileUpload(string command, string workingDirectory, HttpPostedFileBase file, string image)
        {
            try
            {
                return new BaseResponse
                {
                    Status = _dataPortal.FileUpload(command, workingDirectory, file, image),
                    Message = "success"
                };
            }
            catch (Exception e)
            {
                return new BaseResponse
                {
                    Status = ResultStatus.Error,
                    Message = "An error occured"
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
                    Message = "An error occured"
                };
            }
        }
    }
}
