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
            Process[] processes = Process.GetProcessesByName("Arena");
            //CreateSettingsFile();
            //return;

            var settings = Settings.Load("settings.xml");
            Console.WriteLine("Settings loaded.");
            Console.WriteLine("---------------------------------");
            foreach (var item in settings.RestartItems)
            {
                Console.WriteLine("Will close {0} and start {1} at:", item.AppName, item.Location);
                foreach (var time in item.Times)
                {
                    Console.WriteLine(time.ToString());
                }
                Console.WriteLine("Bring to front: {0}", item.BringToFront);
                if (item.BringToFront)
                {
                    Console.WriteLine("Will keep {0} at front.", item.AppName);
                    Thread thread = new Thread(() => KeepForeground(item.AppName));
                    thread.Start();
                }
            }
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Waiting for action...");
            Console.WriteLine("---------------------------------");

            while (true)
            {
                var timeMet = false;
                foreach (var item in settings.RestartItems)
                {
                    foreach (var time in item.Times)
                    {
                        if (DateTime.Now.Hour == time.Hour &&
                            DateTime.Now.Minute == time.Minute &&
                            DateTime.Now.Second == time.Second)
                        {
                            timeMet = true;

                            Console.WriteLine("\n");
                            Thread thread = new Thread(() => ResetProgram(item.AppName, item.Location, item.BringToFront));
                            thread.Start();
                        }
                    }
                }

                if (timeMet)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static void CreateSettingsFile()
        {
            RestartItem item0 = new RestartItem()
            {
                Location = "xxx",
                AppName = "xxx",
                Times = new List<DateTime> { DateTime.Now },
                BringToFront = false,
            };
            RestartItem item1 = new RestartItem()
            {
                Location = "xxx",
                AppName = "xxx",
                Times = new List<DateTime> { DateTime.Now },
                BringToFront = true,
            };

            Settings restartItems = new Settings();
            restartItems.RestartItems.Add(item0);
            restartItems.RestartItems.Add(item1);

            restartItems.Save("Settings.xml");
        }

        private static void ResetProgram(string appName, string location, bool bringToFront)
        {
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Starting to restart {0} at {1}", appName, location);

            Process[] ps = Process.GetProcessesByName(appName);
            if(ps.Length > 0)
            {
                Console.WriteLine("Trying to kill existing process of {0}...", appName);
                while (ps.Length > 0)
                {
                    try
                    {
                        ps.First().Kill();
                    }
                    catch { }
                    Thread.Sleep(1000);
                    ps = Process.GetProcessesByName(appName);
                }
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
                if (bringToFront)
                {
                    Thread.Sleep(10000);
                    try
                    {
                        Console.WriteLine("Bringing {0} to front...", p.ProcessName);
                        var broughtToFront = Pinvoke.SetForegroundWindow(p.MainWindowHandle);
                        Console.WriteLine("Successfully brought to front: {0}", broughtToFront);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Failed to bing to front.");
                        Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to start process at {0}", location);
            }
        }

        private static void KeepForeground(string processName)
        {
            while (true)
            {
                try
                {
                    Process[] ps = Process.GetProcessesByName(processName);
                    if(ps.Length > 0)
                    {
                        Process p = ps.First();
                        Pinvoke.SetForegroundWindow(p.MainWindowHandle);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
                Thread.Sleep(TimeSpan.FromMinutes(5));
                //Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
