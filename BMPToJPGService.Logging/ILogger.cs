using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMPToJPGService.Logging
{
    public interface ILogger
    {
        void Write(LoggingLevel level, string format, params object[] args);
    }
}
