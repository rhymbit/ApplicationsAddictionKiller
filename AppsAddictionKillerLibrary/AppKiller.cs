using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Environment;
using static System.IO.Directory;
using static System.IO.Path;

namespace AppsAddictionKillerLibrary
{
    /// <summary>
    /// Use `RunTimers` method of this class to use the app killing functionality.
    /// Change the `SIZE` const variable to support more number of apps.
    /// </summary>
    public class AppKiller
    {
        private const int SIZE = 10;

        // CLI args for application names(string value),
        // application usetimes(in minutes),
        // application cooldown times(in minutes).
        public static string[] _appsNames = new string[SIZE];
        public static double[] _appsUseTimes = new double[SIZE];
        public static double[] _appsCooldownTimes = new double[SIZE];

        // Max amount time all timers would executed.
        // Must be in milliseconds.
        public static double TimerMaxRunTime { get; set; } = -1;

        /// <summary>
        /// Apps under cooldown time. If running should be killed immediately
        /// </summary>
        public static List<string> _appsKillList = new();

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
        /// Creates logs folder and logs.json file inside it,
        /// if these do not exists.
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
        /// Run this method via a class's instance to use this application.
        /// </summary>
        public void RunTimers()
        {
            CheckCurrentLogsAndCreateNewLogs();
            CreateTimers();
            if (_appsKillList.Count > 0)
            {
                CreateCooldownTimer();
            }
        }

        /// <summary>
        /// Check apps in `_appNames` against their input logs. Allow them to run if
        /// they are over their cooldown time and create new logs for them in `_outputLogs` or
        /// add them to `_appKillList` if they are under cooldown time and replaces these
        /// app's names with "NA" in `_appNames` array.
        /// </summary>
        public void CheckCurrentLogsAndCreateNewLogs()
        {
            for (int i = 0; i < _appsNames.Length; i++)
            {
                if (_appsNames[i] == null)
                {
                    break;
                }
                if (CheckLogsUtility(i) || CheckLogsUtilityNotRunning(i))
                {
                    // This app is allowed to run. Create new logs for this app.
                    CreateNewLogsUtility(i);
                }
                else
                {
                    // Apps under cooldown time are added to this list.
                    AddToKillList(i);

                    // Copy old logs to _output logs for this app,
                    // since app's still under cooldown. Hence logs remain unchanged.
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

                    // Replace app's name in `_appNames` array with "NA"
                    // to avoid starting a specific timer for this app.
                    _appsNames[i] = "NA";

                }
            }
        }

        /// <summary>
        /// Check input against app's current start time.
        /// Returns `true` if app's past its cooldown time.
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
            return false;
        }

        /// <summary>
        /// Check app's which are currently not running against their cooldown times
        /// inside `_inputLogs`. Returns `true` if they are past their cooldown period.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckLogsUtilityNotRunning(int index)
        {
            if (_inputLogs.TryGetValue(_appsNames[index], out string inputCooldownString))
            {
                if (DateTime.TryParse(inputCooldownString, out DateTime inputCooldown))
                {
                    if (DateTime.Now >= inputCooldown)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// App's under cooldown time are added to `_appKillList`,
        /// so if they are started again, they would be killed
        /// until cooldown time expires.
        /// </summary>
        /// <param name="index"></param>
        private static void AddToKillList(int index)
        {
            _appsKillList.Add(_appsNames[index]);
        }

        /// <summary>
        /// Creates new logs for the app's that require it.
        /// Add these new logs to the `_outputLogs`.
        /// </summary>
        /// <param name="index">Position of the app details inside static field arrays.</param>
        public void CreateNewLogsUtility(int index)
        {
            DateTime timeToLog = DateTime.Now.AddMinutes(_appsUseTimes[index]);
            timeToLog = timeToLog.AddMinutes(_appsCooldownTimes[index]);

            if (_outputLogs.TryAdd(_appsNames[index], timeToLog.ToString()))
            { }
            else
            {
                // Remove in production
                Console.WriteLine($"Cannot create logs for {_appsNames[index]}. Error in `CreateNewLogs`");
            }
        }

        /// <summary>
        /// Create kill timers for the app's in `_appNames`. These apps are killed
        /// after their use time expires.
        /// </summary>
        public void CreateTimers()
        {
            for (int i = 0; i < _appsNames.Length; i++)
            {
                if (_appsNames[i] == null)
                {
                    break;
                }
                if (!_appsNames[i].Equals("NA"))
                {
                    timers.Add(SetTimer(i));
                }
            }
        }

        /// <summary>
        /// Create kill timers for the app's in `_appKillList`. These apps are
        /// killed immediately if found running under their cooldown times.
        /// </summary>
        public void CreateCooldownTimer()
        {
            var cooldownTimer = new System.Timers.Timer();
            cooldownTimer.Interval = 5000; // 5 seconds
            cooldownTimer.Elapsed += (sender, e) => CooldownTimerElapsed();
            cooldownTimer.Enabled = true;
            cooldownTimer.AutoReset = true;

            // this will stop the timer and dispose it when program ends
            timers.Add(cooldownTimer);
        }

        /// <summary>
        /// Utility method for `CreateCooldownTimer`. Responsible for killing apps.
        /// </summary>
        public void CooldownTimerElapsed()
        {
            int listSize = _appsKillList.Count;

            int index = new Random().Next(0, listSize);

            var appProcesses = Process.GetProcessesByName(_appsKillList.ElementAt(index));
            foreach (var process in appProcesses)
            {
                if (!process.HasExited)
                {
                    KillApplicationProcess(process);
                }
            }
        }

        /// <summary>
        /// Stops all the timers and dispose their resources.
        /// Must call this method at the very end of the program.
        /// </summary>
        public void DisposeTimers()
        {
            foreach (var timer in timers)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        /// <summary>
        /// Utility method for `CreateTimers` method. This mehod is
        /// Timer_Elapsed method for timers in `CreateTimers` method.
        /// </summary>
        /// <param name="index">App position in static field arrays, for which 
        /// kill timers are generated.</param>
        /// <returns></returns>
        private static System.Timers.Timer SetTimer(int index)
        {
            var timerInterval = _appsUseTimes[index] * 60 * 1000;
            var aTimer = new System.Timers.Timer();
            aTimer.Interval = timerInterval;
            aTimer.Elapsed += (sender, e) => KillApplicationProcessName(index);
            aTimer.AutoReset = false;
            aTimer.Enabled = true; //  Starts the timer
            return aTimer;
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
        /// Utility method, kill the process if found running.
        /// </summary>
        /// <param name="process"></param>
        public static void KillApplicationProcess(Process process)
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
    }
}
