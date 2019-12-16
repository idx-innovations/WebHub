//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace IDX.WebHub.Typing.Configuration
{

    [Serializable]
    [XmlType("Service")]
    public class ServiceConfiguration : Configuration
    {

        #region protected variable(s)

        protected string _extension = null;
        protected Parameters _params = new Parameters();

        #endregion

        #region public properties

        [XmlAttribute("Extension")]
        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }

        public Parameters Parameters
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required for XML serialization</remarks>
        public ServiceConfiguration()
        {
        }

        #endregion

    }

}
