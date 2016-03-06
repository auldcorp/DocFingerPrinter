using DocFingerPrinterBeta.Responses;
using DocFingerPrinterBeta.Static_Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Tests.Static_Classes
{
    [TestClass]
    public class CommandPromptTest
    {
        [TestMethod]
        public void MkDirRmDirTest()
        {
            string command = "mkdir test";
            string rmCommand = "rmdir /S /Q test";
            string workindDir = "C:\\Users\\Public";

            ResultStatus cmStatus = CommandPrompt.ExecuteCommand(command, workindDir);

            Assert.IsNotNull(cmStatus);
            Assert.AreEqual(ResultStatus.Success, cmStatus);
            Assert.IsTrue(Directory.Exists(workindDir + "\\test"));

            //Clean-Up
            ResultStatus rmStatus = CommandPrompt.ExecuteCommand(rmCommand, workindDir);
            Assert.IsNotNull(rmStatus);
            Assert.AreEqual(ResultStatus.Success, rmStatus);
            Assert.IsFalse(Directory.Exists(workindDir + "\\test"));
        }
    }
}
