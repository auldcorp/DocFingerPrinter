using DocFingerPrinterBeta.Responses;
using System.IO;
using System.Drawing;

namespace DocFingerPrinterBeta.Static_Classes
{
    /// <summary>
    /// class to handle all openstego commands/marking and embedding
    /// </summary>
    public static class OpenStego
    {
        /// <summary>Location of Open Stego application</summary>
        public const string OPEN_STEGO_PATH = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";

        /// <summary>Path used to place temporary files</summary>
        public const string WORKING_DIRECTORY = @"C:\Temp\";

        /// <summary>
        /// embeds param embeddedData into image
        /// </summary>
        /// <param name="embeddedData"></param>
        /// <param name="inputData"></param>
        /// <param name="inputDataName"></param>
        /// <returns></returns>
        public static byte[] EmbedData(string embeddedData, byte[] inputData, string inputDataName)
        {
            byte[] result = null;
            string tempTextFilePath = WORKING_DIRECTORY + "tempTextFile.txt";
            string tempImageFilePath = WORKING_DIRECTORY + inputDataName;

            File.WriteAllText(tempTextFilePath, embeddedData);
            File.WriteAllBytes(tempImageFilePath, inputData);
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + tempTextFilePath
               + "\" -cf \"" + tempImageFilePath + "\" -sf \"" + tempImageFilePath + "\"";

            var commandResult = CommandPrompt.ExecuteCommand(embedCommand, WORKING_DIRECTORY);
            if(commandResult == ResultStatus.Success)
            {
                result = File.ReadAllBytes(tempImageFilePath);
            }
            File.Delete(tempTextFilePath);
            File.Delete(tempImageFilePath);
            return result;
        }

        /// <summary>
        /// extracts string data from the file data if the file has previously had data embedded into it
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="inputDataName"></param>
        /// <returns></returns>
        public static string ExtractData(byte[] inputData, string inputDataName)
        {
            string tempImageFilePath = WORKING_DIRECTORY + inputDataName;
            File.WriteAllBytes(tempImageFilePath, inputData);
            string extractCommand = "java -jar " + OPEN_STEGO_PATH + " extract -a RandomLSB -sf \"" + tempImageFilePath + "\" -xd \"" + WORKING_DIRECTORY;
            string result = "";

            ResultStatus r = CommandPrompt.ExecuteCommand(extractCommand, WORKING_DIRECTORY);
            if (r == ResultStatus.Success && File.Exists(WORKING_DIRECTORY + "tempTextFile.txt"))
            {
                result = File.ReadAllText(WORKING_DIRECTORY + "tempTextFile.txt");
                File.Delete(WORKING_DIRECTORY + "tempTextFile.txt");
            }
            File.Delete(tempImageFilePath);
            return result;
        }

        /// <summary>
        /// physically marks text onto the image
        /// </summary>
        /// <param name="corner"></param>
        /// <param name="mark"></param>
        /// <param name="fileBytes"></param>
        /// <param name="transparentSignatureBackground"></param>
        /// <returns></returns>
        public static byte[] WatermarkImage(int corner, string mark, byte[] fileBytes, bool transparentSignatureBackground)
        {
            /*todo: accomodate for buffers on edges when placing watermark
                accomodate for color of image/watermark            
            */
            byte[] bytes;
            using (Stream memStream = new MemoryStream(fileBytes))
            using (Bitmap image = (Bitmap)Image.FromStream(memStream))
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

                if (!transparentSignatureBackground)
                {
                    var rect = new Rectangle(point.X, point.Y, (int)size.Width, (int)size.Height);
                    imageGraphics.FillRectangle(Brushes.White, point.X, point.Y, size.Width, size.Height);
                }
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