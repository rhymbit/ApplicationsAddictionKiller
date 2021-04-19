using System;
using Xunit;
using AppsAddictionKillerLibrary;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestAppKiller
    {
        public TestAppKiller()
        {
            CLIParser.ParseCLIArguments(TD.validCLIArgs);
        }

        [Fact]
        public void TestStaticDataStorageFields()
        {
            Assert.Equal(AppKiller._appsNames.Length/3, TD.validCLIArgs.Length/3);
            Assert.Equal(AppKiller._appsUseTimes.Length/3, TD.validCLIArgs.Length/3);
            Assert.Equal(AppKiller._appsCooldownTimes.Length/3, TD.validCLIArgs.Length/3);
            Assert.NotEqual(-1, AppKiller.TimerMaxRunTime);
        }

        [Fact]
        public void TestKillApplicationProcessName()
        {
            UtilityStartTestProcess();

            System.Threading.Thread.Sleep(5000);

            for (int i = 0; i < TD.validCLIArgs.Length; i++)
            {
                AppKiller.KillApplicationProcessName(i);
            }

            Assert.False(UtilityIsProcessRunning());
        }

        internal static void UtilityStartTestProcess()
        {
            foreach (var testProcess in TD.testProcesses())
            {
                using var tProcess = new Process();
                tProcess.StartInfo.UseShellExecute = true;
                tProcess.StartInfo.FileName = testProcess.Value;
                tProcess.StartInfo.CreateNoWindow = true;
                tProcess.Start();
            }
        }

        internal static bool UtilityIsProcessRunning()
        {
            bool isRunning = false;

            foreach (var testProcess in TD.testProcesses().Keys)
            {
                var tProcess = Process.GetProcessesByName(testProcess);
                foreach (var tp in tProcess)
                {
                    if (!tp.HasExited)
                    {
                        isRunning = true;
                    }
                }
            }
            return isRunning;
        }

        [Fact]
        public void TestCreateCooldownTimer()
        {
            foreach (var item in TD.validCLIArgs)
            {
                AppKiller._appsKillList.Add(item);
            }

            var appKiller = new AppKiller();
            appKiller.CreateCooldownTimer();

            System.Threading.Thread.Sleep(5000);

            appKiller.DisposeTimers();

            Assert.False(UtilityIsProcessRunning());
        }

        internal static void UtilityStartTestProcessIntermittent()
        {
            var testProcesses = TD.testProcesses();
            while (true)
            {
                var rnd = new Random().Next(0, testProcesses.Count);

                using var tProcess = new Process();
                tProcess.StartInfo.UseShellExecute = true;
                tProcess.StartInfo.FileName = testProcesses.Values.ElementAt(rnd);
                tProcess.StartInfo.CreateNoWindow = true;
                tProcess.Start();
                System.Threading.Thread.Sleep(1000);
            }
        }

    }
}