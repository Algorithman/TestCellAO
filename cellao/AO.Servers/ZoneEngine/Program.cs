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
    using System.Threading.Tasks;

    using ZoneEngine.Collision;

    class Program
    {

        public static WallCollision ZoneBorderHandler = new WallCollision();

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

            
                   


        }
    }
}
