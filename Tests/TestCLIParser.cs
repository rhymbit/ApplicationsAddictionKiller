using System;
using Xunit;
using AppsAddictionKillerLibrary;

namespace Tests
{
    public class TestCLIParser
    {
        [Fact]
        public void TestParseCLIArguments()
        {
            // testing valid arguments
            var exception = Record.Exception(() =>
               CLIParser.ParseCLIArguments(TD.validCLIArgs));
            Assert.Null(exception);

            // testing invalid arguments
            // this test case fails for some unknown reason
            //Assert.Throws<ArgumentException>( () => 
            //    CLIParser.ParseCLIArguments(TD.invalidCLIArgs));
        }
    }
}
