using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsAddictionKillerLibrary
{
    public class CLIParser
    {
        public static void ParseCLIArguments(string[] args)
        {
            try
            {
                for(int i=0,j=0; i < args.Length; i+=3, j++)
                {
                    // 0, 3, 6, 9 ... app's names
                    AppKiller._appsNames[j] = args[i];

                    // 1, 4, 7, 10 ... app's use times
                    // Finds the max use time, set it to TimerMaxRuntime
                    if (Double.TryParse(args[i+1],
                        out double useTime))
                    {
                        AppKiller._appsUseTimes[j] = useTime;
                        if (AppKiller.TimerMaxRunTime < useTime)
                        {
                            AppKiller.TimerMaxRunTime = useTime;
                        }

                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                    
                    // 2, 5, 8, 11 ... app's cooldown times
                    if (Double.TryParse(args[i+2],
                        out AppKiller._appsCooldownTimes[j]))
                    {}
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                // Converting 'minutes' to 'milliseconds' for System.Timers.Timer
                AppKiller.TimerMaxRunTime = AppKiller.TimerMaxRunTime * 60 * 1000;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Wrong CLI Arguments!!");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
