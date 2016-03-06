using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Static_Classes
{
    public static class OpenStego
    {
        public static string OPEN_STEGO_PATH = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";

        public static void EmbedData(string embeddedData, string inputFilePath, string outputFilePath)
        {
            string workingDirectory = @"C:\Users\Public";
            File.WriteAllText(workingDirectory + @"\tempTextFile.txt", embeddedData);
            string embedCommand = "java -jar " + OPEN_STEGO_PATH + " embed -a RandomLSB -mf \"" + workingDirectory
               + "\\tempTextFile.txt\" -cf \"" + inputFilePath + "\" -sf \"" + outputFilePath + "\"";
           

            var result = CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);

            File.Delete(workingDirectory + @"\tempTextFile.txt");
        }

        public static void EmbedDataFromFile(string embeddedDataFilePath, string inputFilePath, string outputFilePath)
        {
            string openstegoPath = "\"C:\\Program Files (x86)\\OpenStego\\lib\\openstego.jar\"";
            string embedCommand = "java -jar " + openstegoPath + " embed -a RandomLSB -mf \"" + embeddedDataFilePath
               + "\" -cf \"" + inputFilePath + "\" -sf \"" + outputFilePath + "\"";
            string workingDirectory = @"C:\Users\Public";

            var result = CommandPrompt.ExecuteCommand(embedCommand, workingDirectory);
        }
    }
}