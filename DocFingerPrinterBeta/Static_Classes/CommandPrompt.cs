using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace DocFingerPrinterBeta.Static_Classes
{
    public enum CommandPromptStatus { Success, Error}
    public static class CommandPrompt
    {
        public static CommandPromptStatus ExecuteCommand(string command, string workingDirectory)
        {
            var procedure = new ProcessStartInfo();

            procedure.FileName = @"C:\Windows\System32\cmd.exe";
            procedure.WorkingDirectory = workingDirectory;
            procedure.UseShellExecute = true;
            procedure.Arguments = "/c " + command;
            procedure.WindowStyle = ProcessWindowStyle.Hidden;

            Process.Start(procedure);
            return CommandPromptStatus.Success;
        }
    }
}