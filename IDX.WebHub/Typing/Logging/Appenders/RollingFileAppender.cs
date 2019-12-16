//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

//import namespace(s) required from 'Log4Net' libraries
using log4net;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net.Core;

namespace IDX.WebHub.Typing.Logging.Appenders
{
    public class RollingFileAppender : AppenderBase
    {

        #region "private variable(s)"

        private string _filePath = null;

        private ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _loggingPattern = "%date %level [%thread] - %message%newline";
        private string _maxFileSize = "5MB";
        private int _maxBackupsCount = 3;

        #endregion

        #region "public properties"

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        public RollingFileAppender(string path, Dictionary<string, object> args = null)
        {

            //set the variable(s)
            _filePath = path;

            if (args != null)
            {

                foreach (KeyValuePair<string, object> arg in args)
                {

                    if (arg.Key.ToLower() == "loggingpattern")
                    {

                        _loggingPattern = (string)arg.Value;

                    }

                    if (arg.Key.ToLower() == "maxfilesize")
                    {

                        _maxFileSize = (string)arg.Value;

                    }

                    if (arg.Key.ToLower() == "maxbackupscount")
                    {

                        _maxBackupsCount = (int)arg.Value;

                    }

                }

            }

            //configure 'Log4Net'
            ConfigureLog4Net();

        }

        #endregion

        #region "public method(s), inherited from base-class instance"

        /// <summary>
        /// log writing method
        /// </summary>
        /// <param name="entry"></param>
        public override void Write(LogEntry entry)
        {

            if (entry.Level == LogEntry.InformationLevel.DEBUG)
            {
                _logger.Debug(entry.Message, ParseDetails(entry.Details));
            }

            if (entry.Level == LogEntry.InformationLevel.INFO)
            {
                _logger.Info(entry.Message, ParseDetails(entry.Details));
            }

            if (entry.Level == LogEntry.InformationLevel.WARNING)
            {
                _logger.Warn(entry.Message, ParseDetails(entry.Details));
            }

            if (entry.Level == LogEntry.InformationLevel.EXCEPTION)
            {
                _logger.Error(entry.Message, ParseDetails(entry.Details));
            }

            if (entry.Level == LogEntry.InformationLevel.FATAL)
            {
                _logger.Fatal(entry.Message, ParseDetails(entry.Details));
            }

        }

        #endregion

        #region "private method(s)"

        /// <summary>
        /// method configuring the 'Log4Net' appender
        /// </summary>
        /// <remarks>based on the article found at 'https://stackoverflow.com/questions/16336917/can-you-configure-log4net-in-code-instead-of-using-a-config-file'</remarks>
        private void ConfigureLog4Net()
        {

            //create the output folder (if required)
            string dirPath = _filePath.Substring(0, _filePath.LastIndexOf("\\"));

            if (!Directory.Exists(dirPath))
            {

                Directory.CreateDirectory(dirPath);

            }


            //create the layout pattern
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = _loggingPattern;
            patternLayout.ActivateOptions();

            //enable the rolling file appender
            log4net.Appender.RollingFileAppender roller = new log4net.Appender.RollingFileAppender();

            roller.Layout = patternLayout;

            roller.AppendToFile = true;
            roller.File = _filePath;
            roller.StaticLogFileName = true;

            roller.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = _maxBackupsCount;
            roller.MaximumFileSize = _maxFileSize;

            roller.ActivateOptions();

            //add the appender and configure the 'Log4Net' hierarchy
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(roller);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

        }

        /// <summary>
        /// method parsing the details
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        private Exception ParseDetails(object details)
        {

            if (details != null)
            {

                if (details.GetType().Equals(typeof(Exception)))
                {
                    return (Exception)details;
                }

            }

            return null;

        }

        #endregion

    }
}
