//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'IDX.MEF' library
using IDX.MEF.API;

namespace IDX.WebHub.API.Modules
{
    public class WebModuleMetadata : MetadataBase
    {

        #region "private variable(s)"

        private string _name = String.Empty;

        #endregion

        #region "public properties"

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        public WebModuleMetadata() : base(typeof(IWebModule))
        {
        }

        #endregion

    }
}
