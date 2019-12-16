//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub
{
    public class HostMessageEventArgs : EventArgs
    {

        #region "public enum(s)"

        public enum Types
        {

            DEBUG,
            INFO,
            WARNING,
            EXCEPTION

        }

        #endregion

        #region "private variable(s)"

        private DateTime _timestamp = DateTime.Now;

        #endregion

        #region "public variable(s)"

        public string Message { get; set; }

        public HostMessageEventArgs.Types Type { get; set; }

        public object Details { get; set; }

        #endregion

        #region "public properties"

        public DateTime Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        #endregion

    }
}
