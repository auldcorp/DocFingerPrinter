using DocFingerPrinterBeta.Models;
using DocFingerPrinterBeta.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace DocFingerPrinterBeta.Static_Classes
{
    public static class OpenStego
    {
        public static string OPEN_STEGO_PATH = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";

        public static ResultStatus EmbedData(string embeddedData, string inputFilePath, string outputFilePath)
        {
            string workingDirectory = @"C:\Users\Public";
            File.WriteAllText(workingDirectory + @"\tempTextFile.txt", embeddedData);
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + workingDirectory
               + "\\tempTextFile.txt\" -cf \"" + inputFilePath + "\" -sf \"" + outputFilePath + "\"";
           

            var result = CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);

            File.Delete(workingDirectory + @"\tempTextFile.txt");
            return result;
        }

        public static ResultStatus EmbedDataFromFile(string embeddedDataFilePath, string inputFilePath, string outputFilePath)
        {
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + embeddedDataFilePath
               + "\" -cf \"" + inputFilePath + "\" -sf \"" + outputFilePath + "\"";
            string workingDirectory = @"C:\Users\Public";

            return CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);
        }

        public static void WatermarkImage(int corner, string mark, string inputFilePath, string outputFilePath)
        {
            /*todo: accomodate for buffers on edges when placing watermark
                accomodate for color of image/watermark            
            */
            using (Bitmap image = (Bitmap)System.Drawing.Image.FromFile(inputFilePath))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            {
                Point point;
                if (corner == 0)
                    point = new Point(0, 0);
                else if (corner == 1)
                    //needs to accomodate buffer
                    point = new Point(image.Width - 50, 0);
                else if (corner == 2)
                    //needs to accomodate buffer
                    point = new Point(0, image.Height - 20);
                else if (corner == 3)
                    //needs to accomodate buffer
                    point = new Point(image.Width - 50, image.Height - 20);
                else
                    point = new Point(image.Width / 2, image.Height / 2);

                using (Font arial = new Font("Arial", 10))
                {
                    imageGraphics.DrawString(mark, arial, Brushes.White, point);
                }
                image.Save(outputFilePath);
            }
        }
    }
}