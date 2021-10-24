using BookStoreBackend.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.Services
{
    public class APILogger : ILoggerService
    {
        private static Logger logger;  

        private Logger GetLogger(string LoggerRules)
        {
            if (APILogger.logger == null)
                APILogger.logger = LogManager.GetLogger(LoggerRules);

            return APILogger.logger; 
        }

        public void Debug(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("BookStoreAPILoggerRules").Debug(message);
            else
                GetLogger("BookStoreAPILoggerRules").Debug(message, arg);
        }

        public void Error(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("BookStoreAPILoggerRules").Error(message);
            else
                GetLogger("BookStoreAPILoggerRules").Error(message, arg);
        }

        public void Info(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("BookStoreAPILoggerRules").Info(message);
            else
                GetLogger("BookStoreAPILoggerRules").Info(message, arg);
        }

        public void Warning(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("BookStoreAPILoggerRules").Warn(message);
            else
                GetLogger("BookStoreAPILoggerRules").Warn(message, arg);
        }
    }
}