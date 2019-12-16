//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub.Typing.Logging
{
    public class Log
    {

        #region "public event(s)"

        public event EventHandler<LogEntry> Written;

        #endregion

        #region "private variable(s)"

        private List<LogEntry> _cache = new List<LogEntry>();

        private int _cacheLimit = 100;
        private LogEntry.InformationLevel _threshold = LogEntry.InformationLevel.INFO;

        private List<Appenders.AppenderBase> _appenders = new List<Appenders.AppenderBase>();

        #endregion

        #region "public properties"

        public LogEntry[] Cache
        {
            get
            {
                return _cache.ToArray();
            }
        }

        public int CacheLimit
        {
            get
            {
                return _cacheLimit;
            }
            set
            {
                _cacheLimit = value;
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

        public List<Appenders.AppenderBase> Appenders
        {
            get
            {
                return _appenders;
            }
            set
            {
                _appenders = value;
            }
        }

        #endregion

        #region "static method(s)"

        /// <summary>
        /// method validating a message by an information level threshold given
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool ValidateThreshold(LogEntry entry, LogEntry.InformationLevel threshold)
        {

            if (threshold == LogEntry.InformationLevel.DEBUG)
            {
                return true;
            }

            if (threshold == LogEntry.InformationLevel.INFO)
            {

                if (entry.Level != LogEntry.InformationLevel.DEBUG)
                {
                    return true;
                }

            }

            if (threshold == LogEntry.InformationLevel.WARNING)
            {

                if ((entry.Level != LogEntry.InformationLevel.DEBUG) && (entry.Level != LogEntry.InformationLevel.INFO))
                {
                    return true;
                }

            }

            if (threshold == LogEntry.InformationLevel.EXCEPTION)
            {

                if ((entry.Level == LogEntry.InformationLevel.EXCEPTION) || (entry.Level == LogEntry.InformationLevel.FATAL))
                {
                    return true;
                }

            }

            if (threshold == LogEntry.InformationLevel.FATAL)
            {

                if (entry.Level == LogEntry.InformationLevel.FATAL)
                {
                    return true;
                }

            }

            return false;

        }

        #endregion

        #region "public method(s)"

        /// <summary>
        /// default method for writing a log entry
        /// </summary>
        /// <param name="entry"></param>
        public void Write(LogEntry entry)
        {

            //add the entry
            if (_cacheLimit > 0)
            {

                if (ValidateThreshold(entry, _threshold))
                {

                    _cache.Add(entry);

                }

            }

            //cleanup the cache
            if (_cache.Count > _cacheLimit)
            {

                _cache.RemoveAt(0);

            }

            //write the entry to all appenders
            foreach (Appenders.AppenderBase appender in _appenders)
            {

                appender.Write(entry);

            }

            //raise the notification event
            OnWritten(entry);

        }

        public void Write(string message, LogEntry.InformationLevel level, object details)
        {

            Write(new LogEntry(message, level, details));

        }

        public void Write(string message, LogEntry.InformationLevel level)
        {

            Write(new LogEntry(message, level));

        }

        public void Write(string message)
        {

            Write(new LogEntry(message, LogEntry.InformationLevel.INFO));

        }

        #endregion

        #region "private method(s)"

        /// <summary>
        /// method accepting a message, raising the 'MessageReceived' event
        /// </summary>
        /// <param name="args"></param>
        private void OnWritten(LogEntry entry)
        {

            EventHandler<LogEntry> handler = Written;

            if (handler != null)
            {

                handler(this, entry);

            }

        }

        #endregion

    }
}
