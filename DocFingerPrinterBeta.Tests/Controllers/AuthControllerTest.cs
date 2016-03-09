using DocFingerPrinterBeta.Controllers;
using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DocFingerPrinterBeta.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTest
    {
        [TestMethod]
        public void RegisterTest()
        {

            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User,int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            RegisterModel testModel = new RegisterModel();
            testModel.Email = "blah@gmail.com";
            testModel.Password = "password";
            var result = testController.Register(testModel) as Task<ActionResult>;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SignInTest()
        {
            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            LogInModel testModel = new LogInModel();
            testModel.Email = "blah@gmail.com";
            testModel.Password = "password";
            var result = testController.LogIn(testModel) as Task<ActionResult>;
            Assert.IsTrue(result.IsCompleted);
        }
    }
}
