using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Tesseract;
using System.Web.Hosting;

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
        /// <param name="inputFilePath"></param>
        /// <returns>encodes text</returns>
        public static string getText(string inputFilePath)
        {
            string rootPath = HostingEnvironment.ApplicationPhysicalPath;
            Bitmap image = (Bitmap)System.Drawing.Image.FromFile(inputFilePath);
            TesseractEngine ocr = new TesseractEngine(rootPath, "eng", EngineMode.TesseractOnly);
            ocr.SetVariable("tessedit_char_whitelist", "\\/|#");
            BitmapToPixConverter b = new BitmapToPixConverter();
            Pix p = b.Convert(image);
            Page page = ocr.Process(image, PageSegMode.Auto);
            string text = page.GetText();
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
            int i = 0;
            while (str[i] != '\\')
            {
                i++;
                if (i >= str.Length)
                    return "";
            }
            i++;
            string strReturn = "";
            while (i < str.Length && (str[i] == '|' || str[i] == '/' || str[i] == 'l'))
            {
                strReturn += str[i];
                i++;
            }
            if (strReturn == "" && i < str.Length)
                return getUserIDString(str);
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
            int i = 0;
            while (str[i] != '#')
            {
                i++;
                if (i >= str.Length)
                    return "";
            }
            i++;
            string strReturn = "";
            while (i < str.Length && (str[i] == '|' || str[i] == '/' || str[i] == 'l'))
            {
                strReturn += str[i];
                i++;
            }
            if (strReturn == "" && i < str.Length)
                return getImageIDString(strReturn);
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
                    case 'l':
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