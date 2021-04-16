using System;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;
using static System.Environment;
using static System.IO.Directory;

namespace Tests
{
    public static class TD
    {
        public static string[] validCLIArgs =
        {
            "py" , "12" , "20",
            "msedge", "14", "25",
            "chrome" , "20" , "23"
        };

        public static string[] invalidCLIArgs =
        {
            "23" , "12" , "20",
            "msedge", "hello", "25",
            "chrome" , "20" , "kitty"
        };

        public static string logsFolderPath = Combine(
            GetFolderPath(SpecialFolder.MyDocuments), "AppsAddictionKillerLogs");

        public static string logsFilePath = Combine(
            logsFolderPath, "logs.json");
        
        
    }
}