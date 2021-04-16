using System;
using AppsAddictionKillerLibrary;

namespace AppsAddictionKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            var _ = new AppKiller();
            var logsManager = new LogsManager();
            logsManager.ReadInputLogs();
        }
    }
}
