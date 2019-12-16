//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub.Typing.Logging.Appenders
{
    public abstract class AppenderBase
    {

        public abstract void Write(LogEntry entry);

    }
}
