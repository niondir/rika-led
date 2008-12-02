using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using StoreServer.Data;
using StoreServer.WebService;

namespace StoreServer
{
    class Program
    {
        private static Thread httpServiceThread;
        private static Thread thread;
        private static Process process;
        private static Assembly assembly;
        private static string baseDirectory;
        private static string exePath;
        private static UserManager userManager;
        private static DataManager dataManager;

        private static bool unix = false;
        public static bool Unix { get { return unix; } }


        /// <summary>
        /// if true, the programm is shutting down, used for threads to stop
        /// </summary>
        private static bool closing = false;
        public static bool Closing { get { return closing; } }

        public static Assembly Assembly { get { return assembly; } set { assembly = value; } }
        public static Process Process { get { return process; } }
        public static Thread Thread { get { return thread; } }
        public static UserManager UserManager { get { return userManager; } }
        public static DataManager DataManager { get { return dataManager; } }

        public static string ExePath
        {
            get
            {
                if (exePath == null)
                {
                    exePath = Assembly.Location;
                }

                return exePath;
            }
        }

        public static string BaseDirectory
        {
            get
            {
                if (baseDirectory == null)
                {
                    try
                    {
                        baseDirectory = ExePath;

                        if (baseDirectory.Length > 0)
                            baseDirectory = Path.GetDirectoryName(baseDirectory);
                    }
                    catch
                    {
                        baseDirectory = "";
                    }
                }
                return baseDirectory;
            }
        }

        private static AutoResetEvent signal = new AutoResetEvent( true );
        public static void Set() { signal.Set(); }


        static void Main(string[] args)
        {
            thread = Thread.CurrentThread;
			process = Process.GetCurrentProcess();
			assembly = Assembly.GetEntryAssembly();

            if( thread != null )
				thread.Name = "Core Thread";

            if( BaseDirectory.Length > 0 )
				Directory.SetCurrentDirectory( BaseDirectory );

            Version ver = assembly.GetName().Version;
            
            // Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
            Console.WriteLine("StoreServer - [http://code.google.com/p/rika-led/]");
            Console.WriteLine("Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
			Console.WriteLine("Server: Running on .NET Framework Version {0}.{1}.{2}", Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build);
           
            int platform = (int)Environment.OSVersion.Platform;
            if ((platform == 4) || (platform == 128))
            { // MS 4, MONO 128
                unix = true;
                Console.WriteLine("Server: Unix environment detected");
            }
 
            HttpService httpService = new HttpService("http://127.0.0.1:11000/", new ClientHandler());
            
            userManager = new UserManager();
            dataManager = new DataManager();

            try
            {

                while (signal.WaitOne())
                {
                    httpService.Slice();
                    dataManager.Slice();
                    Console.WriteLine("--- New round! ---");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // TODO: Loop for async actions, linke console input
            Console.WriteLine("Server: Pres any key to exit");
            Console.ReadKey();
            closing = true;
        }
    }

}