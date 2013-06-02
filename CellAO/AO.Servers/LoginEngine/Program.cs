#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
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
#endregion

namespace LoginEngine
{
    #region Usings ...

    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using AO.Core;
    using AO.Core.Components;
    using AO.Core.Logger;
    using AO.Database.Dao;
    using AO.Database.Entities;

    using LoginEngine.CoreServer;

    using MySql.Data.MySqlClient;

    using NBug;
    using NBug.Properties;

    using NLog;

    using Config = AO.Core.Config.ConfigReadWrite;

    #endregion

    /// <summary>
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        private static readonly IContainer Container = new MefContainer();

        /// <summary>
        /// </summary>
        private static LoginServer loginServer;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool Ismixed()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return info[0] == "1";
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool Ismodified()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return info[1] == "1";
        }

        /// <summary>
        /// </summary>
        /// <param name="emailAddress">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool TestEmailRegex(string emailAddress)
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

        /// <summary>
        /// </summary>
        /// <param name="args">
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private static void Main(string[] args)
        {
            LogUtil.SetupConsoleLogging(LogLevel.Debug);
            LogUtil.SetupFileLogging("${basedir}/LoginEngineLog.txt", LogLevel.Trace);



            SettingsOverride.LoadCustomSettings("NBug.LoginEngine.Config");
            Settings.WriteLogToDisk = true;
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;



            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description
                            + " " + AssemblyInfoclass.AssemblyVersion + " " + AssemblyInfoclass.Trademark;

            var ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.Write("Loading ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(AssemblyInfoclass.Title + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(AssemblyInfoclass.Description);
            Console.ResetColor();
            Console.WriteLine("...");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ResetColor();

            // Sying helped figure all this code out, about 5 yearts ago! :P
            bool processedargs = false;
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
                string temp = string.Empty;
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
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length < 9)
                            {
                                Console.WriteLine(
                                    "Invalid command syntax.\nPlease use:\nAdduser <username> <password> <number of characters> <expansion> <gm level> <email> <FirstName> <LastName>");
                                break;
                            }

                            string username = parts[1];
                            string password = parts[2];
                            int numChars = 0;
                            try
                            {
                                numChars = int.Parse(parts[3]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <number of characters> must be a number (duh!)");
                                break;
                            }

                            int expansions = 0;
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

                            int gm = 0;
                            try
                            {
                                gm = int.Parse(parts[5]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <GM Level> must be number (duh!)");
                                break;
                            }

                            string email = parts[6];
                            if (email == null)
                            {
                                email = string.Empty;
                            }

                            if (!TestEmailRegex(email))
                            {
                                Console.WriteLine("Error: <Email> You must supply an email address for this account");
                                break;
                            }

                            string firstname = parts[7];
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

                            string lastname = parts[8];
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

                            DBLoginData login = new DBLoginData
                                                    {
                                                        Username=username,
                                                        AccountFlags = 0,
                                                        Allowed_Characters = numChars,
                                                        CreationDate = DateTime.Now,
                                                        Email = email,
                                                        Expansions = expansions,
                                                        FirstName = firstname,
                                                        LastName = lastname,
                                                        GM = gm,
                                                        Flags = 0,
                                                        Password = new LoginEncryption().GeneratePasswordHash(password)
                                                    };
                            try
                            {
                                LoginDataDao.WriteLoginData(login);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    "An error occured while trying to add a new user account:\n{0}", ex.Message);
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
                            string Syntax =
                                "The Syntax for this command is \"hash <String to hash>\" alphanumeric no spaces";
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length != 2)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }

                            string pass = parts[1];
                            var le = new LoginEncryption();
                            string hashed = le.GeneratePasswordHash(pass);
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
                            string Syntax =
                                "The syntax for this command is \"setpass <account username> <newpass>\" where newpass is alpha numeric no spaces";
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length != 3)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }

                            string username = parts[1];
                            string newpass = parts[2];
                            var le = new LoginEncryption();
                            string hashed = le.GeneratePasswordHash(newpass);

                            try
                            {
                                LoginDataDao.WriteNewPassword(
                                    new DBLoginData { Username = username, Password = hashed });
                            }
                                // yeah this part here, some kind of exception handling for mysql errors
                            catch (Exception ex)
                            {
                                Console.WriteLine("Could not set new Password\r\n"+ex.Message);
                                LogUtil.ErrorException(ex);
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