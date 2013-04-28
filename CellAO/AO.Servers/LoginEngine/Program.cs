﻿using System;

using System.Threading.Tasks;

namespace LoginEngine
{
    using System.Diagnostics;
    using System.Net;
    using System.Text.RegularExpressions;

    using AO.Core;
    using AO.Core.Components;
    using MySql.Data.MySqlClient;

    using LoginEngine.CoreServer;

    using NBug;
    using NBug.Properties;

    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Config = AO.Core.Config.ConfigReadWrite;
    class Program
    {


        #region Static Fields

        private static readonly
            IContainer Container = new MefContainer();

        private static
        LoginServer loginServer;

        #endregion

        #region Public Methods and Operators

        public static
        bool Ismixed
        ()
        {
            var info = AssemblyInfoclass.Trademark.Split(';');
            return info[0] == "1";
        }

        public static
            bool Ismodified
            ()
        {
            var info = AssemblyInfoclass.Trademark.Split(';');
            return info[1] == "1";
        }

        public static
            bool TestEmailRegex
            (string
            emailAddress)
        {
            const string PatternStrict =
                @"^(([^<>()[\]\\.,;:\s@\""]+" + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}" + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                + @"[a-zA-Z]{2,}))$";

            var reStrict = new Regex(PatternStrict);
            return reStrict.IsMatch(emailAddress);
        }

        #endregion

        #region Methods

        private static
            void Main
            (string[]
            args)
        {
            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: "
                            + AssemblyInfoclass.Description + " " + AssemblyInfoclass.AssemblyVersion + " "
                            + AssemblyInfoclass.Trademark;

            var ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ResetColor();

            // Sying helped figure all this code out, about 5 yearts ago! :P
            var processedargs = false;
            loginServer = Container.GetInstance<LoginServer>();
            bool TCPEnable = true;
            bool UDPEnable = false;
            int Port = Convert.ToInt32(Config.Instance.CurrentConfig.LoginPort);
            try
            {
                if (Config.Instance.CurrentConfig.ListenIP == "0.0.0.0")
                {
                    loginServer.TcpEndPoint = new IPEndPoint(IPAddress.Any, Port);
                }
                else
                {
                    loginServer.TcpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
                }
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }



            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/LoginEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            var rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            var rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;

            SettingsOverride.LoadCustomSettings("NBug.LoginEngine.Config");
            NBug.Settings.WriteLogToDisk = true;
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;

            // TODO: ADD More Handlers.
            loginServer.MaximumPendingConnections = 100;

            #region Console Commands

            // Andyzweb: Added checks for start and stop
            // also added a running command to return status of the server
            // and added Console.Write("\nServer Command >>"); to login server
            string consoleCommand;
            ct.TextRead("login_consolecommands.txt");
            while (true)
            {
                if (!processedargs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            loginServer.Start(TCPEnable, UDPEnable);
                        }
                    }

                    processedargs = true;
                }

                Console.Write("\nServer Command >>");

                consoleCommand = Console.ReadLine();
                var temp = string.Empty;
                while (temp != consoleCommand)
                {
                    temp = consoleCommand;
                    consoleCommand = consoleCommand.Replace("  ", " ");
                }

                consoleCommand = consoleCommand.Trim();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        if (loginServer.IsRunning)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            ct.TextRead("loginisrunning.txt");
                            Console.ResetColor();
                            break;
                        }

                        loginServer.Start(TCPEnable, UDPEnable);
                        break;
                    case "stop":
                        if (!loginServer.IsRunning)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            ct.TextRead("loginisnotrunning.txt");
                            Console.ResetColor();
                            break;
                        }

                        loginServer.Stop();
                        break;
                    case "exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "running":
                        if (loginServer.IsRunning)
                        {
                            // Console.WriteLine("Login Server is running");
                            ct.TextRead("loginisrunning.txt");
                            break;
                        }

                        // Console.WriteLine("Login Server not running");
                        ct.TextRead("loginisnotrunning.txt");
                        break;

                    #region Help Commands....

                    case "help":
                        ct.TextRead("logincmdhelp.txt");
                        break;
                    case "help start":
                        ct.TextRead("helpstart.txt");
                        break;
                    case "help exit":
                        ct.TextRead("helpstop.txt");
                        break;
                    case "help running":
                        ct.TextRead("loginhelpcmdrunning.txt");
                        break;
                    case "help Adduser":
                        ct.TextRead("logincmdadduserhelp.txt");
                        break;
                    case "help setpass":
                        ct.TextRead("logincmdhelpsetpass.txt");
                        break;

                    #endregion

                    default:

                        #region Adduser

                        // This section handles the command for adding a user to the database
                        if (consoleCommand.ToLower().StartsWith("adduser"))
                        {
                            var parts = consoleCommand.Split(' ');
                            if (parts.Length < 9)
                            {
                                Console.WriteLine(
                                    "Invalid command syntax.\nPlease use:\nAdduser <username> <password> <number of characters> <expansion> <gm level> <email> <FirstName> <LastName>");
                                break;
                            }

                            var username = parts[1];
                            var password = parts[2];
                            var numChars = 0;
                            try
                            {
                                numChars = int.Parse(parts[3]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <number of characters> must be a number (duh!)");
                                break;
                            }

                            var expansions = 0;
                            try
                            {
                                expansions = int.Parse(parts[4]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <expansions> must be a number between 0 and 2047!");
                                break;
                            }

                            if (expansions < 0 || expansions > 2047)
                            {
                                Console.WriteLine("Error: <expansions> must be a number between 0 and 2047!");
                                break;
                            }

                            var gm = 0;
                            try
                            {
                                gm = int.Parse(parts[5]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <GM Level> must be number (duh!)");
                                break;
                            }

                            var email = parts[6];
                            if (email == null)
                            {
                                email = string.Empty;
                            }

                            if (!TestEmailRegex(email))
                            {
                                Console.WriteLine(
                                    "Error: <Email> You must supply an email address for this account");
                                break;
                            }

                            var firstname = parts[7];
                            try
                            {
                                if (firstname == null)
                                {
                                    throw new ArgumentNullException();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <FirstName> You must supply a first name for this accout");
                                break;
                            }

                            var lastname = parts[8];
                            try
                            {
                                if (lastname == null)
                                {
                                    throw new ArgumentNullException();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <LastName> You must supply a last name for this account");
                                break;
                            }

                            const string FormatString =
                                "INSERT INTO `login` (`CreationDate`, `Flags`,`AccountFlags`,`Username`,`Password`,`Allowed_Characters`,`Expansions`, `GM`, `Email`, `FirstName`, `LastName`) VALUES "
                                + "(NOW(), '0', '0', '{0}', '{1}', {2}, {3}, {4}, '{5}', '{6}', '{7}');";

                            var le = new LoginEncryption();

                            var hashedPassword = le.GeneratePasswordHash(password);

                            var sql = string.Format(
                                FormatString,
                                username,
                                hashedPassword,
                                numChars,
                                expansions,
                                gm,
                                email,
                                firstname,
                                lastname);
                            var sqlWrapper = new SqlWrapper();
                            try
                            {
                                sqlWrapper.SqlInsert(sql);
                            }
                            catch (MySqlException ex)
                            {
                                switch (ex.Number)
                                {
                                    case 1062: // duplicate entry for key
                                        Console.WriteLine("A user account with this username already exists.");
                                        break;
                                    default:
                                        Console.WriteLine(
                                            "An error occured while trying to add a new user account:\n{0}",
                                            ex.Message);
                                        break;
                                }

                                break;
                            }

                            Console.WriteLine("User added successfully.");
                            break;
                        }

                        #endregion

                        #region Hashpass

                        // This function just hashes the string you enter using the loginencryption method
                        if (consoleCommand.ToLower().StartsWith("hash"))
                        {
                            var Syntax =
                                "The Syntax for this command is \"hash <String to hash>\" alphanumeric no spaces";
                            var parts = consoleCommand.Split(' ');
                            if (parts.Length != 2)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }

                            var pass = parts[1];
                            var le = new LoginEncryption();
                            var hashed = le.GeneratePasswordHash(pass);
                            Console.WriteLine(hashed);
                            break;
                        }

                        #endregion

                        #region setpass

                        // sets the password for the given username
                        // Added by Andyzweb
                        // Still TODO add exception and error handling
                        if (consoleCommand.ToLower().StartsWith("setpass"))
                        {
                            var Syntax =
                                "The syntax for this command is \"setpass <account username> <newpass>\" where newpass is alpha numeric no spaces";
                            var parts = consoleCommand.Split(' ');
                            if (parts.Length != 3)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }

                            var username = parts[1];
                            var newpass = parts[2];
                            var le = new LoginEncryption();
                            var hashed = le.GeneratePasswordHash(newpass);
                            string formatString;
                            formatString = "UPDATE `login` SET Password = '{0}' WHERE login.Username = '{1}'";

                            var sql = string.Format(formatString, hashed, username);

                            var updt = new SqlWrapper();
                            try
                            {
                                updt.SqlUpdate(sql);
                            }



                                // yeah this part here, some kind of exception handling for mysql errors
                            catch
                            {
                            }
                        }

                        #endregion

                        ct.TextRead("login_consolecmdsdefault.txt");
                        break;
                }
            }

            #endregion
        }

        #endregion

    }
}

