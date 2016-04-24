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
    public class OpenStegoTest
    {
        [TestMethod]
        public void EmbedDataAndExtractTest()
        {
            string embededData = "test";
            string inputFilePath = "C:\\Users\\Public\\small-mario.png";
            byte[] fileBytes = File.ReadAllBytes(inputFilePath);

            byte[] embeddedData = OpenStego.EmbedData(embededData, fileBytes, Path.GetFileName(inputFilePath));
            Assert.IsNotNull(embeddedData);

            string extractedText = OpenStego.ExtractData(embeddedData, Path.GetFileName(inputFilePath));
            Assert.AreEqual(extractedText, "test");

        }

        [TestMethod]
        public void WaterMarkImageTopLeftTest()
        {
            string inputFilePath = "C:\\Users\\Public\\test.png";
            string outputFilePath = "C:\\Users\\Public\\topleft.png";

            //OpenStego.WatermarkImage(0, "test", inputFilePath, outputFilePath);

            //assert that image was actually watermarked

        }

        [TestMethod]
        public void WaterMarkImageTopRightTest()
        {
            string inputFilePath = "C:\\Users\\Public\\test.png";
            string outputFilePath = "C:\\Users\\Public\\topright.png";

            //OpenStego.WatermarkImage(1, "test", inputFilePath, outputFilePath);

        }

        [TestMethod]
        public void WaterMarkImageBottomLeftTest()
        {
            string inputFilePath = "C:\\Users\\Public\\test.png";
            string outputFilePath = "C:\\Users\\Public\\bottomleft.png";

            //OpenStego.WatermarkImage(2, "test", inputFilePath, outputFilePath);

        }

        [TestMethod]
        public void WaterMarkImageBottomRightTest()
        {
            string inputFilePath = "C:\\Users\\Public\\test.png";
            string outputFilePath = "C:\\Users\\Public\\bottomright.png";

            //OpenStego.WatermarkImage(3, "test", inputFilePath, outputFilePath);

        }
    }
}
