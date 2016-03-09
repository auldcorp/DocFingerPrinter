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

        public void RegisterTestWithoutPassword()
        {
            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            RegisterModel testModel = new RegisterModel();
            testModel.Email = "blah@gmail.com";
            var result = testController.Register(testModel) as Task<ActionResult>;
            Assert.IsTrue(result.IsFaulted);
        }

        public void RegisterTestEmpty()
        {
            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            RegisterModel testModel = new RegisterModel();
            var result = testController.Register(testModel) as Task<ActionResult>;
            Assert.IsTrue(result.IsFaulted);
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

        [TestMethod]
        public void SignInTestEmpty()
        {
            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            LogInModel testModel = new LogInModel();
            var result = testController.LogIn(testModel) as Task<ActionResult>;
            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public void SignInTestWithoutPassword()
        {
            var mockStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(mockStore.Object);
            AuthController testController = new AuthController(userManager);
            LogInModel testModel = new LogInModel();
            testModel.Email = "blah@gmail.com";
            var result = testController.LogIn(testModel) as Task<ActionResult>;
            Assert.IsTrue(result.IsCompleted);
        }
    }
}
