using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Tesseract;
using System.Web.Hosting;

namespace DocFingerPrinterBeta.Static_Classes
{
    public class TesseractDetection
    {
        public static string getText(string inputFilePath)
        {
            string rootPath = HostingEnvironment.ApplicationPhysicalPath;
            Bitmap image = (Bitmap)System.Drawing.Image.FromFile(inputFilePath);
            TesseractEngine ocr = new TesseractEngine(rootPath, "eng", EngineMode.TesseractOnly);
            BitmapToPixConverter b = new BitmapToPixConverter();
            Pix p = b.Convert(image);
            Page page = ocr.Process(image, PageSegMode.Auto);
            string text = page.GetText();
            return text;
        }

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
        
        //Returns an array of the userID followed by the imageID
        public static string convertFullMarkToString(string str)
        {
            string userIDStr = getUserIDString(str);
            string imageIDStr = getImageIDString(str);
            int userIDInt = convertToInt(userIDStr);
            int imageIDInt = convertToInt(imageIDStr);
            string result = (userIDInt + "#" + imageIDInt);
            return result;
        }

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