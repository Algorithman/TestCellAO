﻿// Copyright (c) 2005-2013, CellAO Team
//  
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//  
//      * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//      * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//      * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//  
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
  
using System;
using AO.Core.Components;
using NBug;
using NBug.Properties;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ZoneEngine
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using AO.Core;
    using ZoneEngine.Collision;
    using ZoneEngine.CoreClient;
    using ZoneEngine.CoreServer;
    using ZoneEngine.Script;
    using Config = AO.Core.Config.ConfigReadWrite;

    class Program
    {
        #region Static Fields
        
        private static readonly
        IContainer Container = new MefContainer();
        
        private static
        ZoneServer zoneServer;
        
        #endregion
        
        public static WallCollision ZoneBorderHandler = new WallCollision();
        
        public static ScriptCompiler csc;
        
        static void Main(string[] args)
        {
            bool processedargs = false;

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
            
            zoneServer = Container.GetInstance<ZoneServer>();
            bool TCPEnable = true;
            bool UDPEnable = false;
            int Port = Convert.ToInt32(Config.Instance.CurrentConfig.ZonePort);
            try
            {
                if (Config.Instance.CurrentConfig.ListenIP == "0.0.0.0")
                {
                    zoneServer.TcpEndPoint = new IPEndPoint(IPAddress.Any, Port);
                }
                else
                {
                    zoneServer.TcpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
                }
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }

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
        
        public static void StartTheServer()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                /* TODO: Readd the things, Algorithman
                zoneServer.Monsters = new List<NonPlayerCharacterClass>();
                zoneServer.Vendors = new List<VendingMachine>();
                zoneServer.Doors = new List<Doors>();
                
                using (SqlWrapper sqltester = new SqlWrapper())
                {
                if (sqltester.SQLCheck() != SqlWrapper.DBCheckCodes.DBC_ok)
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Database setup not correct");
                Console.WriteLine("Error: #" + sqltester.lasterrorcode + " - " + sqltester.lasterrormessage);
                Console.WriteLine("Please press Enter to exit.");
                Console.ReadLine();
                Process.GetCurrentProcess().Kill();
                }
                sqltester.CheckDBs();
                }
                */
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Loaded {0} items", ItemHandler.CacheAllItems());
                Console.WriteLine("Loaded {0} nanos", NanoHandler.CacheAllNanos());
                Console.WriteLine("Loaded {0} spawns", NonPlayerCharacterHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} vendors", VendorHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} teleports", DoorHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} statels", Statels.CacheAllStatels());
                
                /* Same as above
                LootHandler.CacheAllFromDB();
                Tradeskill.CacheItemNames();
                */
                csc.AddScriptMembers();
                csc.CallMethod("Init", null);
                
                zoneServer.Start(true, false);
                Console.ResetColor();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("MySql Error. Server Cannot Start");
                Console.WriteLine("Exception: " + e.Message);
                string current = DateTime.Now.ToString("HH:mm:ss");
                StreamWriter logfile = File.AppendText("ZoneEngineLog.txt");
                logfile.WriteLine(current + " " + e.Source + " MySql Error. Server Cannot Start");
                logfile.WriteLine(current + " " + e.Source + " Exception: " + e.Message);
                logfile.Close();
                zoneServer.Stop();
                ThreadMgr.Stop();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}