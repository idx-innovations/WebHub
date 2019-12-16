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
    [XmlType("StaticContent")]
    public class StaticContentConfiguration : Configuration
    {

        #region protected variable(s)

        protected string _path = null;
        protected string _route = null;

        #endregion

        #region public properties

        [XmlAttribute("Path")]
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        [XmlAttribute("Route")]
        public string Route
        {
            get
            {
                return _route;
            }
            set
            {
                _route = value;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required for XML serialization</remarks>
        public StaticContentConfiguration()
        {
        }

        #endregion

    }

}
