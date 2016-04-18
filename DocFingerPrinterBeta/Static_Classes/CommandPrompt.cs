using DocFingerPrinterBeta.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace DocFingerPrinterBeta.Static_Classes
{
    /// <summary>
    /// class that handles execution of command prompt processes
    /// </summary>
    public static class CommandPrompt
    {

        /// <summary>
        /// executes param command by spawning a process and inputting a specified command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static ResultStatus ExecuteCommand(string command, string workingDirectory)
        {
            var procedure = new ProcessStartInfo();

            procedure.FileName = @"C:\Windows\System32\cmd.exe";
            procedure.WorkingDirectory = workingDirectory;
            procedure.UseShellExecute = true;
            procedure.Arguments = "/c " + command;
            procedure.WindowStyle = ProcessWindowStyle.Hidden;

            var process = new Process() { StartInfo = procedure };
            process.Start();

            //Should probably have this timeout and return error
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Dispose();

            if (exitCode != 0)
                return ResultStatus.Error;

            return ResultStatus.Success;
        }
    }
}