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
        public void getTextTest()
        {
            string imagePath = Directory.GetCurrentDirectory() + "\\Images\\Profile\\bottomright.png";
            string str = TesseractDetection.getText(imagePath);
            str = TesseractDetection.removeWhiteSpaces(str);
            str = TesseractDetection.removeNewLineCharacters(str);
            Assert.IsNotNull(str);
            Assert.AreEqual("\\|/|//#|/|", str);
        }

        [TestMethod]
        public void getUserIDTest()
        {
            string imagePath = Directory.GetCurrentDirectory() + "\\Images\\Profile\\landscapeMarked.png";
            string str = TesseractDetection.getText(imagePath);
            str = TesseractDetection.getUserIDString(str);
            Assert.IsNotNull(str);
            Assert.AreEqual("|/|//", str);
        }

        [TestMethod]
        public void getImageIDTest()
        {
            string imagePath = Directory.GetCurrentDirectory() + "\\Images\\Profile\\landscapeMarked.png";
            string str = TesseractDetection.getText(imagePath);
            str = TesseractDetection.getImageIDString(str);
            Assert.IsNotNull(str);
            Assert.AreEqual("|/|", str);
        }

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
    }
}
