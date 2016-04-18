﻿using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DocFingerPrinterBeta.Static_Classes
{
    /// <summary>
    /// class to handle all openstego commands/marking and embedding
    /// </summary>
    public static class OpenStego
    {
        public static string OPEN_STEGO_PATH = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";

        /// <summary>
        /// embeds param embeddedData into image
        /// </summary>
        /// <param name="embeddedData"></param>
        /// <param name="inputData"></param>
        /// <param name="inputDataName"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public static byte[] EmbedData(string embeddedData, byte[] inputData, string inputDataName, string outputFilePath)
        {
            byte[] result = null;
            string workingDirectory = @"C:\Users\Public\";
            string tempTextFilePath = workingDirectory + @"tempTextFile.txt";
            string tempImageFilePath = workingDirectory + inputDataName;

            File.WriteAllText(tempTextFilePath, embeddedData);
            File.WriteAllBytes(tempImageFilePath, inputData);
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + tempTextFilePath
               + "\" -cf \"" + tempImageFilePath + "\" -sf \"" + outputFilePath + "\"";

            var commandResult = CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);
            if(commandResult == ResultStatus.Success)
            {
                result = File.ReadAllBytes(tempImageFilePath);
            }
            File.Delete(tempTextFilePath);
            File.Delete(tempImageFilePath);
            return result;
        }

        /// <summary>
        /// embeds data in embeddedDataFilePath into the image
        /// </summary>
        /// <param name="embeddedDataFilePath"></param>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public static ResultStatus EmbedDataFromFile(string embeddedDataFilePath, string inputFilePath, string outputFilePath)
        {
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + embeddedDataFilePath
               + "\" -cf \"" + inputFilePath + "\" -sf \"" + outputFilePath + "\"";
            string workingDirectory = @"C:\Users\Public";

            return CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);
        }

        /// <summary>
        /// extracts data from the file if the files has previously had data embedded from it
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public static string ExtractDataFromFile(string inputFilePath)
        {
            string workingDirectory = @"C:\Users\Public";
            string extractCommand = "java -jar " + OPEN_STEGO_PATH + " extract -a RandomLSB -sf \"" + inputFilePath + "\" -xd \"" + workingDirectory + "\\data\"";
            string result = "";

            ResultStatus r = CommandPrompt.ExecuteCommand(extractCommand, workingDirectory);
            if (r == ResultStatus.Success && File.Exists(workingDirectory + "\\data\\tempTextFile.txt"))
            {
                result = System.IO.File.ReadAllText(workingDirectory + "\\data\\tempTextFile.txt");
                File.Delete(workingDirectory + "\\data\\tempTextFile.txt");
            }
            return result;
        }

        /// <summary>
        /// physically marks text onto the image
        /// </summary>
        /// <param name="corner"></param>
        /// <param name="mark"></param>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public static byte[] WatermarkImage(int corner, string mark, string inputFilePath)
        {
            /*todo: accomodate for buffers on edges when placing watermark
                accomodate for color of image/watermark            
            */
            byte[] bytes;

            using (Bitmap image = (Bitmap)System.Drawing.Image.FromFile(inputFilePath))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (Font font = new Font("Sans", 40))
            {
                imageGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                Point point = new Point(0, 0);
                var size = imageGraphics.MeasureString(mark, font);

                if (corner == 1)
                    point = new Point(image.Width - (int)size.Width, 0);
                else if (corner == 2)
                    point = new Point(0, image.Height - (int)size.Height);
                else if (corner == 3)
                    point = new Point(image.Width - (int)size.Width, image.Height - (int)size.Height);

                //var rect = new Rectangle(point.X, point.Y, (int)size.Width, (int)size.Height);
                imageGraphics.FillRectangle(Brushes.White, point.X, point.Y, size.Width, size.Height);
                imageGraphics.DrawString(mark, font, Brushes.Black, point);

                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    bytes = stream.ToArray();
                }
                //give option to maximize contrast of text and or choose color of text/box
            }
            return bytes;
        }
    }
}