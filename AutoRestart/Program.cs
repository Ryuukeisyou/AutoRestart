using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace AutoRestart
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //CreateSettingsFile();
            var settings = Settings.Load("settings.xml");
            Console.WriteLine("Settings loaded. Waiting for action.");

            while (true)
            {
                foreach (var item in settings.RestartItems)
                {
                    foreach (var time in item.Times)
                    {
                        if (DateTime.Now.Hour == time.Hour &&
                            DateTime.Now.Minute == time.Minute &&
                            DateTime.Now.Second == time.Second)
                        {
                            Console.WriteLine(DateTime.Now);
                            Console.WriteLine("Starting to restart {0} at {1}", item.AppName, item.Location);
                            ResetProgram(item.AppName, item.Location);
                        }
                    }

                }
                Thread.Sleep(1000);
            }

        }

        private static void CreateSettingsFile()
        {
            RestartItem item0 = new RestartItem()
            {
                Location = "xxx",
                AppName = "xxx",
                Times = new List<DateTime> { DateTime.Now },
            };
            RestartItem item1 = new RestartItem()
            {
                Location = "xxx",
                AppName = "xxx",
                Times = new List<DateTime> { DateTime.Now },
            };

            Settings restartItems = new Settings();
            restartItems.RestartItems.Add(item0);
            restartItems.RestartItems.Add(item1);

            restartItems.Save("Settings.xml");
        }

        private static void ResetProgram(string appName, string location)
        {
            Process[] ps = Process.GetProcessesByName(appName);
            if(ps.Length > 0)
            {
                Console.WriteLine("Trying to kill existing process of {0}...", appName);
                ps.First().Kill();
                Console.WriteLine("Process killed.");
            }
            else
            {
                Console.WriteLine("No process of {0} exists.");
            }

            Process p = new Process();
            p.StartInfo.FileName = location;
            
            try
            {
                p.Start();
                Console.WriteLine("Successfully started process at {0}.", location);
            }
            catch
            {
                Console.WriteLine("Failed to start process at {0}", location);
            }
        }
    }
}
