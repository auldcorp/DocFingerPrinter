using DocFingerPrinterBeta.DataLayer;
using DocFingerPrinterBeta.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Tests.DataLayer
{
    [TestClass]
    public class DataPortalTest
    {
        [TestMethod]
        public void GetUsersTest()
        {
            //integration test
            //setup
            DataPortal dataPortalClass = new DataPortal();
            List<User> users = dataPortalClass.GetUsers();
            Assert.IsNotNull(users);
        }
    }
}
