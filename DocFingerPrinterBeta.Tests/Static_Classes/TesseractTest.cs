using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocFingerPrinterBeta.Static_Classes;
using System.IO;

namespace DocFingerPrinterBeta.Tests.Static_Classes
{
    [TestClass]
    public class TesseractTest
    {

        [TestMethod]
        public void convertToIntTest()
        {
            string str = "|||/|/";
            int actual = TesseractDetection.convertToInt(str);
            int expected = 58;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void convertToIntErrorTest()
        {
            string str = "|/dkd|/";
            int actual = TesseractDetection.convertToInt(str);
            int expected = 0;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void convertToIntZeroStartTest()
        {
            string str = "/|///||||/";
            int actual = TesseractDetection.convertToInt(str);
            int expected = 286;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void convertFullMarkToStringTest()
        {
            string str = "\\|///||||/#|||/|/";
            string actual = TesseractDetection.convertFullMarkToString(str);
            string expected = "286#58";
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
