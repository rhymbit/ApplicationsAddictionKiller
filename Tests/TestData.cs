using System;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;
using static System.Environment;
using static System.IO.Directory;

namespace Tests
{
    public class TD
    {
        public static string[] validCLIArgs =
        {
            "py" , "2" , "1",
            "msedge", "3", "2",
            "notepad" , "4" , "3"
        };

        public static string[] invalidCLIArgs =
        {
            "23" , "2" , "1",
            "msedge", "hello", "2",
            "notepad" , "4" , "kitty"
        };

        public static Dictionary<string, string> testProcesses()
        {
            Dictionary<string,string> temp = new()
            {
                { validCLIArgs[0], pythonTestProcessPath },
                { validCLIArgs[3], validCLIArgs[3] },
                { validCLIArgs[6], validCLIArgs[6] }
            };
            return temp;
        }

        // Replace this string value with the path of `testProcess.py` in the top level directory of the Tests folder.
        public static string pythonTestProcessPath = "C:\\Users\\prateek_parashar\\code\\VisualStudio\\" +
                        "\\AppsAddictionKiller\\Tests\\testProcess.py";

        public static string logsFolderPath = Combine(
            GetFolderPath(SpecialFolder.MyDocuments), "AppsAddictionKillerLogs");

        public static string logsFilePath = Combine(
            logsFolderPath, "logs.json");

    }
}