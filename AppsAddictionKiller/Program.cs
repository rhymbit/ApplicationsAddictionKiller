using System;
using AppsAddictionKillerLibrary;

namespace AppsAddictionKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse CLI args, start timers for each app, sleep until each timer runs out,
            // dispose timers.

            var IpOpLogsReader = new LogsManager();
            IpOpLogsReader.ReadInputLogs();

            CLIParser.ParseCLIArguments(args);
            var appKiller = new AppKiller();
            appKiller.RunTimers();

            IpOpLogsReader.WriteOutputLogs();

            System.Threading.Thread.Sleep((int)AppKiller.TimerMaxRunTime + 1000);
            appKiller.DisposeTimers();
        }
    }
}
