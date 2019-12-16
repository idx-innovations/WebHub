//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub.Typing.Logging
{
    public class LogEntry
    {

        #region "public variable(s)"

        public enum InformationLevel { DEBUG, INFO, WARNING, EXCEPTION, FATAL };

        #endregion

        #region "private variable(s)"

        private DateTime _timestamp = DateTime.Now;

        private string _message = null;
        private InformationLevel _level = InformationLevel.INFO;
        private object _details = null;

        #endregion

        #region "public properties"

        public InformationLevel Level
        {
            get
            {
                return _level;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return _timestamp;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public object Details
        {
            get
            {
                return _details;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="details"></param>
        public LogEntry(string message, InformationLevel level, object details)
        {

            //set the variable(s)
            _level = level;
            _message = message;
            _details = details;

        }

        /// <summary>
        /// overloaded new instance method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public LogEntry(string message, InformationLevel level)
        {

            //set the variable(s)
            _level = level;
            _message = message;

        }

        #endregion

    }
}
