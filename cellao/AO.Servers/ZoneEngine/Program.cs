using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBug;
using NBug.Properties;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ZoneEngine
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using AO.Core;

    using ZoneEngine.Collision;
    using ZoneEngine.Script;
    using ZoneEngine.CoreClient;

    class Program
    {
        public static WallCollision ZoneBorderHandler = new WallCollision();

        public static ScriptCompiler csc;

        static void Main(string[] args)
        {
            #region NLog
            
            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/ZoneEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;

            #endregion
            
            #region NBug
            
            SettingsOverride.LoadCustomSettings("NBug.ZoneEngine.Config");
            NBug.Settings.WriteLogToDisk = true;
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;
            //TODO: ADD More Handlers.
            
            #endregion

            #region Console Text...
            
            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description +
                            " " + AssemblyInfoclass.AssemblyVersion + " " + AssemblyInfoclass.Trademark;
            
            ConsoleText ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Using ISComm v1.0");
            Console.WriteLine("[OK]");
            Console.ResetColor();
            
            #endregion
            
            #region Console Commands...
                
            string consoleCommand;
            ct.TextRead("zone_consolecommands.txt");
            // removed CheckDBs here, added commands check and updatedb (updatedb will change to a versioning 
                        
            while (true)
            {
                if (!processedargs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            csc.Compile(false);
                            StartTheServer();
                        }
                    }
                    processedargs = true;
                }
                Console.Write("\nServer Command >>");
                consoleCommand = Console.ReadLine();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is already running");
                            Console.ResetColor();
                            break;
                        }
                        
                        //TODO: Add Sql Check.
                        csc.Compile(false);
                        StartTheServer();
                        break;
                    case "startm": // Multiple dll compile
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is already running");
                            Console.ResetColor();
                            break;
                        }
                        
                        //TODO: Add Sql Check.
                        csc.Compile(true);
                        StartTheServer();
                        break;
                    case "stop":
                        if (!zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is not running");
                            Console.ResetColor();
                            break;
                        }
                        zoneServer.Stop();
                        ThreadMgr.Stop();
                        break;
                    case "check":
                    case "updatedb":
                        using (SqlWrapper sqltester = new SqlWrapper())
                        {
                            sqltester.CheckDBs();
                            Console.ResetColor();
                        }
                        break;
                    case "exit":
                    case "quit":
                        if (zoneServer.Running)
                        {
                            zoneServer.Stop();
                            ThreadMgr.Stop();
                        }
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "ls": //list all available scripts, dont remove it since it does what it should
                        Console.WriteLine("Available scripts");
                        /* Old Lua way
                        string[] files = Directory.GetFiles("Scripts");*/
                        string[] files = Directory.GetFiles("Scripts\\", "*.cs", SearchOption.AllDirectories);
                        if (files.Length == 0)
                        {
                            Console.WriteLine("No scripts were found.");
                            break;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        foreach (string s in files)
                        {
                            Console.WriteLine(s);
                            /* Old Lua way
                            if (s.EndsWith(".lua"))
                            {
                            Console.WriteLine(s.Split('\\')[1].Split('.')[0]);
                            }*/
                        }
                        Console.ResetColor();
                        break;
                    case "ping":
                        // ChatCom.Server.Ping();
                        Console.WriteLine("Ping is disabled till we can fix it");
                        break;
                    case "running":
                        if (zoneServer.Running)
                        {
                            Console.WriteLine("Zone Server is Running");
                            break;
                        }
                        Console.WriteLine("Zone Server not Running");
                        break;
                    case "online":
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            lock (zoneServer.Clients)
                            {
                                foreach (Client c in zoneServer.Clients)
                                {
                                    Console.WriteLine("Character " + c.Character.Name + " online");
                                }
                            }
                            Console.ResetColor();
                        }
                        break;
                    default:
                        ct.TextRead("zone_consolecmdsdefault.txt");
                        break;
                }
            }
        }

        #endregion
    }
}