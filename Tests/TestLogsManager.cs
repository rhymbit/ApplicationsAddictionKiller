using System;
using Xunit;
using AppsAddictionKillerLibrary;
using System.IO;

namespace Tests
{
	public class TestLogsManager
	{
		[Fact]
		public void TestReadInputLogs()
        {
			var _ = new AppKiller();
			var logsManager = new LogsManager();
			Assert.True(logsManager.ReadInputLogs());
        }

		[Fact]
		public void TestWriteOutputLogs()
        {
			CLIParser.ParseCLIArguments(TD.validCLIArgs);
			var appKiller = new AppKiller();
			appKiller.CreateNewLogsUtility(1);
			var logsManager = new LogsManager();
			logsManager.WriteOutputLogs();

			var fileContents = File.ReadAllText(TD.logsFilePath);
			Assert.NotEqual(0, fileContents.Length);
        }
	}
}
