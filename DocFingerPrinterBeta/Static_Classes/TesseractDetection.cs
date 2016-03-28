using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Tesseract;

namespace DocFingerPrinterBeta.Static_Classes
{
    public class TesseractDetection
    {
        public static string getText(string inputFilePath)
        {
            Bitmap image = (Bitmap)System.Drawing.Image.FromFile(inputFilePath);
            TesseractEngine ocr = new TesseractEngine("tessdata", "eng");
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
    }
}