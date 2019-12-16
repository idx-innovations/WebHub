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
    [XmlType("Properties")]
    public class PropertiesConfiguration : Configuration
    {

        #region protected variable(s)

        protected Parameters _params = new Parameters();

        #endregion

        #region public properties

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
        public PropertiesConfiguration()
        {
        }

        #endregion

    }

}
