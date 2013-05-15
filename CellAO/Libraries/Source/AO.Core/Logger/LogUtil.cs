using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace AO.Core.Logger
{
    using System.Diagnostics;
    using System.Threading;

    using NLog.Config;
    using NLog.Targets;

    using Logger = NLog.Logger;

    public static class LogUtil
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static Action<Action<string>> SystemInfoLogger;

        public static event Action<string, Exception> ExceptionRaised;


        public static void SetupConsoleLogging(LogLevel logLevel)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget
            {
                Layout = "${processtime} [${level}] ${message} ${exception:format=tostring}"
            };
            config.AddTarget("console", consoleTarget);

            config.LoggingRules.Add(new LoggingRule("*", logLevel, consoleTarget));

            LogManager.Configuration = config;
            LogManager.EnableLogging();
        }

        public static void SetupFileLogging(string fileName, LogLevel logLevel)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();
            var fileTarget = new FileTarget();
            fileTarget.FileName = fileName;
            config.AddTarget("logfile", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", logLevel, fileTarget));
            LogManager.Configuration = config;
            LogManager.EnableLogging();
        }

        public static void Debug(string msg)
        {
            log.Debug(msg);
        }

        #region Exceptions
        public static void ErrorException(Exception e)
        {
            ErrorException(e, false);
        }

        public static void ErrorException(Exception e, bool addSystemInfo)
        {
            ErrorException(e, addSystemInfo, "");
        }

        public static void ErrorException(string msg, params object[] format)
        {
            ErrorException(false, msg, format);
        }

        public static void ErrorException(bool addSystemInfo, string msg, params object[] format)
        {
            LogException(log.Error, null, addSystemInfo, msg, format);
        }

        public static void ErrorException(Exception e, string msg, params object[] format)
        {
            ErrorException(e, true, msg, format);
        }

        public static void ErrorException(Exception e, bool addSystemInfo, string msg, params object[] format)
        {
            LogException(log.Error, e, addSystemInfo, msg, format);
        }

        public static void WarnException(Exception e)
        {
            WarnException(e, false);
        }

        public static void WarnException(Exception e, bool addSystemInfo)
        {
            WarnException(e, addSystemInfo, "");
        }

        public static void WarnException(string msg, params object[] format)
        {
            WarnException(false, msg, format);
        }

        public static void WarnException(bool addSystemInfo, string msg, params object[] format)
        {
            LogException(log.Warn, null, addSystemInfo, msg, format);
        }

        public static void WarnException(Exception e, string msg, params object[] format)
        {
            WarnException(e, true, msg, format);
        }

        public static void WarnException(Exception e, bool addSystemInfo, string msg, params object[] format)
        {
            LogException(log.Warn, e, addSystemInfo, msg, format);
        }

        public static void FatalException(Exception e, string msg, params object[] format)
        {
            FatalException(e, true, msg, format);
        }

        public static void FatalException(Exception e, bool addSystemInfo)
        {
            FatalException(e, addSystemInfo, "");
        }

        public static void FatalException(Exception e, bool addSystemInfo, string msg, params object[] format)
        {
            LogException(log.Fatal, e, addSystemInfo, msg, format);
        }

        public static void LogException(Action<string> logger, Exception e, bool addSystemInfo, string msg, params object[] format)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format(msg, format);
                logger(msg);
            }

            if (e != null)
            {
                LogStacktrace(logger);
                logger("");
                logger(e.ToString());
                //logger(new StackTrace(e, true));
            }

            if (addSystemInfo)
            {
                logger("");
                if (SystemInfoLogger != null)
                {
                    SystemInfoLogger(logger);
                }
                else
                {
                    LogSystemInfo(logger);
                }
            }

            var evt = ExceptionRaised;
            if (evt != null)
            {
                evt(msg, e);
            }
        }
        #endregion

        public static void LogStacktrace(Action<string> logger)
        {
            StackTrace stackTrace = new StackTrace(Thread.CurrentThread, true);
            string temp = "";
            foreach (StackFrame stackFrame in stackTrace.GetFrames())
            {
                temp += stackFrame.ToString().Trim() + "\r\n\t";
            }
            logger(temp);
        }

        private static void LogSystemInfo(Action<string> logger)
        {
            var title = "WCell component";
#if DEBUG
            title += " - Debug";
#else
			title += " - Release";
#endif
            logger(title);
            logger(string.Format("OS: {0} - CLR: {1}", Environment.OSVersion, Environment.Version));
        }
    }
}
