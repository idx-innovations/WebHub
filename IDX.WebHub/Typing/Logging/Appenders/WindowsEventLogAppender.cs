//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace IDX.WebHub.Typing.Logging.Appenders
{
    public class WindowsEventLogAppender : AppenderBase
    {

        #region "private variable(s)"

        private string _log = "Application";
        private string _source = null;

        private LogEntry.InformationLevel _threshold = LogEntry.InformationLevel.INFO;

        #endregion

        #region "public properties"

        public string LogName
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        public string SourceName
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public LogEntry.InformationLevel Threshold
        {
            get
            {
                return _threshold;
            }
            set
            {
                _threshold = value;
            }
        }

        #endregion

        #region "public new instance method(s)"

        public WindowsEventLogAppender(Dictionary<string, object> args = null)
        {

            if (args != null)
            {

                foreach (KeyValuePair<string, object> arg in args)
                {

                    if (arg.Key.ToLower() == "logname")
                    {

                        _log = (string)arg.Value;

                    }

                    if (arg.Key.ToLower() == "sourcename")
                    {

                        _source = (string)arg.Value;

                    }

                    if (arg.Key.ToLower() == "threshold")
                    {

                        _threshold = (LogEntry.InformationLevel)arg.Value;

                    }

                }

            }

        }

        #endregion

        #region "public method(s), inherited from base-class instance"

        /// <summary>
        /// log writing method
        /// </summary>
        /// <param name="entry"></param>
        public override void Write(LogEntry entry)
        {

            if (Log.ValidateThreshold(entry, _threshold))
            {

                if ((entry.Level == LogEntry.InformationLevel.DEBUG) || (entry.Level == LogEntry.InformationLevel.INFO))
                {

                    WriteToEventLog(entry.Message, EventLogEntryType.Information);

                }

                if (entry.Level == LogEntry.InformationLevel.WARNING)
                {

                    string msg = entry.Message;

                    if (entry.Details != null)
                    {

                        if (entry.Details.GetType().Equals(typeof(Exception)))
                        {

                            Exception details = (Exception)entry.Details;
                            msg += "\n" + details.Message + "\n" + "\n" + details.StackTrace;

                        }

                    }

                    WriteToEventLog(msg, EventLogEntryType.Warning);

                }

                if ((entry.Level == LogEntry.InformationLevel.EXCEPTION) || (entry.Level == LogEntry.InformationLevel.FATAL))
                {

                    string msg = entry.Message;

                    if (entry.Details != null)
                    {

                        if (entry.Details.GetType().Equals(typeof(Exception)))
                        {

                            Exception details = (Exception)entry.Details;
                            msg += "\n" + details.Message + "\n" + "\n" + details.StackTrace;

                        }

                    }

                    WriteToEventLog(msg, EventLogEntryType.Error);

                }

            }

        }

        #endregion

        #region "private method(s)"

        private void WriteToEventLog(string message, EventLogEntryType type)
        {

            //create the source if required
            if (!EventLog.SourceExists(_source))
            {
                EventLog.CreateEventSource(_source, _log);
            }

            //add the log entry
            EventLog myLog = new EventLog(_log);
            myLog.Source = _source;
            myLog.WriteEntry(message, type);

        }

        #endregion

    }
}
