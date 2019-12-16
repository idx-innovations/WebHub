//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace IDX.WebHub.Typing.Configuration
{

    /// <summary>
    /// abstract static content definition class
    /// </summary>
    /// <remarks>please see the article at 'https://code-examples.net/de/q/b55ed2' for details on the usage of the 'XmlInclude' attribute</remarks>
    [Serializable]
    [XmlInclude(typeof(ServiceConfiguration)),XmlInclude(typeof(StaticContentConfiguration)),XmlInclude(typeof(PropertiesConfiguration))]
    public abstract class Configuration
    {

        #region protected variable(s)

        protected string _id = null;

        #endregion

        #region public properties

        [XmlAttribute("ID")]
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        #endregion

    }

}
