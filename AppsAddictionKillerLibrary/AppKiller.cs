using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using static System.IO.Path;
using static System.Environment;
using static System.IO.Directory;
using System.Collections.Concurrent;

namespace AppsAddictionKillerLibrary
{
    public class AppKiller
    {
        private const int SIZE = 10;

        // CLI Arguments
        public static string[] _appsNames = new string[SIZE];
        public static double[] _appsUseTimes = new double[SIZE];
        public static double[] _appsCooldownTimes = new double[SIZE];

        // Max amount time all timers would executed
        public static double TimerMaxRunTime { get; set; } = -1;

        // Logs Folder and File Path
        static readonly string _logsFolderPath = Combine(
                GetFolderPath(SpecialFolder.MyDocuments), "AppsAddictionKillerLogs");
        internal static readonly string _logsFilePath = Combine(
                _logsFolderPath, "logs.json");

        // Input Logs, read from logs file
        internal static Dictionary<string, string> _inputLogs = new();
        // OutputLogs, write to logs file
        internal static Dictionary<string, string> _outputLogs = new();

        // Store timers created for killing apps so later those timers could be disposed.
        private static List<System.Timers.Timer> timers = new List<System.Timers.Timer>();

        /// <summary>
        /// Creates logs folder and logs json file.
        /// </summary>
        public AppKiller()
        {
            if (!Exists(_logsFolderPath))
            {
                CreateDirectory(_logsFolderPath);
            }
            if (!File.Exists(_logsFilePath))
            {
                File.CreateText(_logsFilePath);
            }
        }

        /// <summary>
        /// Kills a process from array `_appsName[index]` by using Process's name.
        /// </summary>
        /// <param name="index">Position of process to kill in array.</param>
        public static void KillApplicationProcessName(int index)
        {
            // all running process for app
            var appProcesses = Process.GetProcessesByName(_appsNames[index]);

            foreach (var app in appProcesses)
            {
                if (!app.HasExited)
                {
                    app.Kill();
                }
            }
        }

        /// <summary>
        /// Kills a process (does not kill its child processes)
        /// </summary>
        /// <param name="appProcess">Process to kill</param>
        public void KillApplicationProcess(Process appProcess)
        {
            if (!appProcess.HasExited)
            {
                appProcess.Kill();
            }
        }

        /// <summary>
        /// Kill apps that are still under cooldown time
        /// </summary>
        public void CheckCurrentLogsAndCreateNewLogs()
        {
            for (int i = 0; i < _appsNames.Length; i++)
            {
                if (CheckLogsUtility(i))
                {
                    // This app is allowed to run. Create new logs for this app.
                    CreateNewLogsUtility(i);
                }
                else
                {
                    // This app is still under its cooldown period, so kill it.
                    KillApplicationProcessName(i);
                    // Copy old logs to _output logs for this app.
                    if (_inputLogs.TryGetValue(_appsNames[i], out string logTimeString))
                    {
                        if (_outputLogs.TryAdd(_appsNames[i], logTimeString))
                        { }
                        else
                        {
                            Console.WriteLine("_inputLogs value corrupted");
                            throw new ArgumentException();
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Check input against app's current start time.
        /// Returns `true` if app's passed its cooldown time.
        /// </summary>
        /// <param name="index">App's name position in `_appNames` array.</param>
        /// <returns></returns>
        public bool CheckLogsUtility(int index)
        {
            var appProcesses = Process.GetProcessesByName(_appsNames[index]);
            
            foreach (var appProcess in appProcesses)
            {
                if (_inputLogs.TryGetValue(appProcess.ProcessName, out string inputCooldownString))
                {
                    if (DateTime.TryParse(inputCooldownString, out DateTime inputCooldown))
                    {
                        if (appProcess.StartTime >= inputCooldown)
                        {
                            // App's over its previous set cooldown time
                            return true;
                        }
                        else
                        {
                            // App's under its previous set cooldown time
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void CreateNewLogsUtility(int index)
        {
            DateTime timeToLog = DateTime.Now.AddMinutes(_appsUseTimes[index]);
            timeToLog = timeToLog.AddMinutes(_appsCooldownTimes[index]);

            if (_outputLogs.TryAdd(_appsNames[index], timeToLog.ToString()))
            {}
            else
            {
                // Remove in production
                Console.WriteLine($"Cannot create logs for {_appsNames[index]}. Error in `CreateNewLogs`");
            }
        }

        public void CreateTimers()
        {
            for(int i = 0; i < _appsNames.Length; i++)
            {
                if (!_appsNames[i].Equals("NA"))
                {
                    timers.Add(SetTimer(i));
                }
            }
        }
        public void DisposeTimers()
        {
            foreach (var timer in timers)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
        
        private static System.Timers.Timer SetTimer(int index)
        {
            var timerInterval = _appsUseTimes[index] * 60 * 1000;
            var aTimer = new System.Timers.Timer(timerInterval);
            aTimer.Elapsed += (sender, e) => KillApplicationProcessName(index);
            aTimer.AutoReset = false;
            aTimer.Enabled = true; //  Starts the timer
            return aTimer;
        }

    }

}
