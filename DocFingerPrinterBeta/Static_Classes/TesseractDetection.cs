using System;
using System.Drawing;
using Tesseract;
using System.Web.Hosting;
using System.IO;

namespace DocFingerPrinterBeta.Static_Classes
{
    /// <summary>
    /// class to handle all tesseract library calls
    /// </summary>
    public class TesseractDetection
    {
        /// <summary>
        /// pulls marked from param inputFilePath image
        /// </summary>
        /// <param name="fileBytes">Byte array of file data</param>
        /// <returns>encodes text</returns>
        public static string getText(byte[] fileBytes)
        {
            string text = "", rootPath = HostingEnvironment.ApplicationPhysicalPath;
            BitmapToPixConverter b = new BitmapToPixConverter();

            using (Stream memStream = new MemoryStream(fileBytes))
            using (Bitmap image = (Bitmap)Image.FromStream(memStream))
            using (TesseractEngine ocr = new TesseractEngine(rootPath, "eng", EngineMode.TesseractOnly))
            {

                image.SetResolution(300, 300);
                ocr.SetVariable("tessedit_char_whitelist", "\\/|#");
                Pix p = b.Convert(image);
                p = p.ConvertRGBToGray();
                Page page = ocr.Process(p, PageSegMode.Auto);
                text = page.GetText();
                p.Dispose();
                page.Dispose();
            }

            return text;
        }

        /// <summary>
        /// takes the string str and removes any whitespaces
        /// </summary>
        /// <param name="str"></param>
        /// <returns>param str minus whitespaces</returns>
        public static string removeWhiteSpaces(string str)
        {
            string strReturn = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ' && str[i] != '\t')
                    strReturn += str[i];
            }
            return strReturn;
        }

        /// <summary>
        /// takes the string str and removes any new lines characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns>param str minus newlines characters</returns>
        public static string removeNewLineCharacters(string str)
        {
            string strReturn = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '\n')
                    strReturn += str[i];
            }
            return strReturn;
        }

        /// <summary>
        /// takes the encoded string param str and returns the user Id associated with it
        /// </summary>
        /// <param name="str"></param>
        /// <returns>the users id associated with the encoded string str</returns>
        public static string getUserIDString(string str)
        {
            string strReturn = "";
            if (!string.IsNullOrEmpty(str))
            {
                int startIndex = str.IndexOf('\\') + 1;
                int endIndex = str.IndexOf('#');

                if( startIndex != -1 && endIndex != -1 && startIndex < endIndex)
                    strReturn = str.Substring(startIndex, endIndex - startIndex);
            }
            return strReturn;
        }

        /// <summary>
        /// Returns an array of the userID followed by the imageID
        /// </summary>
        /// <param name="str"></param>
        /// <returns>an array of the userdId followed by the imageId</returns>
        public static string convertFullMarkToString(string str)
        {
            string userIDStr = getUserIDString(str);
            string imageIDStr = getImageIDString(str);
            int userIDInt = convertToInt(userIDStr);
            int imageIDInt = convertToInt(imageIDStr);
            string result = (userIDInt + "#" + imageIDInt);
            return result;
        }

        /// <summary>
        /// takes the encoded string param str and returns the image Id associated with it
        /// </summary>
        /// <param name="str"></param>
        /// <returns>imageId associated with the encoding</returns>
        public static string getImageIDString(string str)
        {
            string strReturn = "";
            if (!string.IsNullOrEmpty(str) && str.IndexOf('#') != -1)
            {
                int startIndex = str.IndexOf('#') + 1;
                int endIndex = startIndex;
   
                while (endIndex < str.Length && (str[endIndex] == '|' || str[endIndex] == '/'))
                {
                    endIndex++;
                }
                strReturn = str.Substring(startIndex, endIndex - startIndex);
            }

            return strReturn;
        }


        /// <summary>
        /// converts integer to binary signature
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string convertIntToBinarySignature(int x)
        {
            string result = "";
            string binary = Convert.ToString(x, 2);
            for (int i=0; i < binary.Length; i++)
            {
                if (binary[i] == '0')
                    result += "/";
                else
                    result += "| ";
            }
            return result;
        }

        /// <summary>
        /// converts encoded string to integers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //Pass in only valued characters from the marking scheme
        public static int convertToInt(string str)
        {
            int value = 0;
            int b = 0;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '|':
                        b = 1;
                        break;
                    case '/':
                        b = 0;
                        break;
                    default:
                        return 0;
                }
                int exponent = str.Length - i - 1;
                value += b * (int) (Math.Pow(2, exponent));
            }
            return value;
        }
    }
}