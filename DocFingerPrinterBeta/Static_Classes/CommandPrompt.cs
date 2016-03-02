using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace DocFingerPrinterBeta.Static_Classes
{
    public enum CommandPromptExitStatus { Success, Error}
    public static class CommandPrompt
    {
        public static CommandPromptExitStatus ExecuteCommand(string command, string workingDirectory)
        {
            var procedure = new ProcessStartInfo();

            procedure.FileName = @"C:\Windows\System32\cmd.exe";
            procedure.WorkingDirectory = workingDirectory;
            procedure.UseShellExecute = true;
            procedure.Arguments = "/c " + command;
            procedure.WindowStyle = ProcessWindowStyle.Hidden;

            var process = new Process() { StartInfo = procedure };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
                return CommandPromptExitStatus.Error;

            return CommandPromptExitStatus.Success;
        }
    }
}