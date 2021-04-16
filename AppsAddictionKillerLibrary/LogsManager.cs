using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AppsAddictionKillerLibrary
{
    public class LogsManager
    {
        public bool ReadInputLogs()
        {
            if (File.Exists(AppKiller._logsFilePath))
            {
                try
                {
                    var appsLogsJson = File.ReadAllText(AppKiller._logsFilePath);

                    // if logs file is empty, then logs haven't been created yet
                    if (appsLogsJson.Length == 0)
                    {
                        return true;
                    }

                    AppKiller._inputLogs = JsonSerializer.Deserialize
                        <Dictionary<string, string>>(appsLogsJson);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Logs file does not exists");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create or write output logs to `MyDocuments` folder.
        /// </summary>
        /// <returns></returns>
        public async void WriteOutputLogs()
        {
            using (StreamWriter writer = File.CreateText(AppKiller._logsFilePath))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var jsonOutputLogsData = JsonSerializer.Serialize
                    <Dictionary<string, string>>(AppKiller._outputLogs, options);

                await writer.WriteAsync(jsonOutputLogsData);
            }
        }
    }
}
