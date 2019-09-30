using System;
using LoanDepot.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace BMPToJPGService.Logging
{
    class LogToConsole : ILogger
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        public void Write(LoggingLevel level, string format, params object[] args)
        {
            Console.Write(level.ToString().ToUpper());
            Console.Write("[" + DateTime.Now.ToString() + "]");
            Console.Write(": ");
            Console.WriteLine(format, args);

            var message = string.Format(format, args);
            switch (level)
            {
                case LoggingLevel.Debug:
                    log.Debug(message);
                    break;
                case LoggingLevel.Information:
                    log.Info(message);
                    break;
                case LoggingLevel.Warning:
                    log.Warn(message);
                    break;
                case LoggingLevel.Error:
                    log.Error(message);
                    break;
            }
        }
    }
}
