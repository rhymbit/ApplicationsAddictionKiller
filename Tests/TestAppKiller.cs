using System;
using Xunit;
using AppsAddictionKillerLibrary;
using System.Diagnostics;

namespace Tests
{
    public class TetsAppKiller
    {
        [Fact]
        public void TestStaticFields()
        {

            CLIParser.ParseCLIArguments(TD.validCLIArgs);

            Assert.Equal(AppKiller._appsNames.Length/3, TD.validCLIArgs.Length/3);
            Assert.Equal(AppKiller._appsUseTimes.Length/3, TD.validCLIArgs.Length/3);
            Assert.Equal(AppKiller._appsCooldownTimes.Length/3, TD.validCLIArgs.Length/3);
            Assert.NotEqual(-1, AppKiller.TimerMaxRunTime);
        }

        [Fact]
        public void TestKillApplicationProcessName()
        {
            CLIParser.ParseCLIArguments(TD.validCLIArgs);
            
            for (int i = 0; i < TD.validCLIArgs.Length; i++)
            {
                AppKiller.KillApplicationProcessName(i);
            }

            bool isRunning = false;

            for (int i = 0; i < TD.validCLIArgs.Length; i++)
            {
                var myProcess = Process.GetProcessesByName(
                    TD.validCLIArgs[i]);
                foreach (var mp in myProcess)
                {
                    if (!mp.HasExited)
                    {
                        isRunning = true;
                    }
                }
            }
            Assert.False(isRunning);
        }
    }
}